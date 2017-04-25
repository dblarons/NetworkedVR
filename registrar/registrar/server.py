import flatbuffers
import uuid
import zmq

import registrar.Registrar.Command
import registrar.Registrar.Create
import registrar.Registrar.Join
import registrar.Registrar.Message

from registrar.models.client import Client
from registrar.models.room import Room
import registrar.api.connect as connect_api
import registrar.api.create as create_api
import registrar.api.join as join_api
import registrar.api.list as list_api
import registrar.config as config

SUB_PORT = '5555'
REP_PORT = '5556'

db = config.database
db.connect();

context = zmq.Context()

# Create the response socket.
rep_socket = context.socket(zmq.REP)
rep_socket.bind('tcp://*:%s' % REP_PORT)

# Create the publisher socket.
pub_socket = context.socket(zmq.PUB)
pub_socket.bind('tcp://*:%s' % SUB_PORT)

def build_command(builder, message_type, message):
    registrar.Registrar.Command.CommandStart(builder)
    registrar.Registrar.Command.CommandAddMessageType(
        builder, message_type)
    registrar.Registrar.Command.CommandAddMessage(builder, message)
    return registrar.Registrar.Command.CommandEnd(builder)

while True:
    #  Wait for next request from client
    request = rep_socket.recv()

    Command = registrar.Registrar.Command.Command.GetRootAsCommand(request, 0)

    # Switch on the message type to send a response.
    req_builder = flatbuffers.Builder(1024)
    pub_builder = flatbuffers.Builder(1024)
    message_offset = 0
    message_type = Command.MessageType()
    publish_offset = None
    channel = None
    if message_type == registrar.Registrar.Message.Message().List:
        print('SERVER: Received a List command')
        rooms = Room.select()
        message_offset = list_api.response(req_builder, rooms)
    elif message_type == registrar.Registrar.Message.Message().Create:
        print('SERVER: Received a Create command')
        union_create = registrar.Registrar.Create.Create()
        union_create.Init(Command.Message().Bytes, Command.Message().Pos)
        client = union_create.Client()
        room = None
        with db.transaction():
            room = Room.create(guid=str(uuid.uuid1()), name=union_create.Name())
            Client.create(
                internal_name=client.Id(),
                ip=client.Ip(),
                room=room.id,
                port=client.Port())

        message_offset = create_api.response(req_builder, room)
    elif message_type == registrar.Registrar.Message.Message().Join:
        print('SERVER: Received a Join command')
        union_join = registrar.Registrar.Join.Join()
        union_join.Init(Command.Message().Bytes, Command.Message().Pos)
        guid = union_join.Guid()
        client = union_join.Client()
        room = Room.get(Room.guid == guid)

        if room is None:
            print('ERROR: No room matching given guid was found')
        else:
            # Send room and existing clients to the requester.
            message_offset = join_api.response(req_builder, room, room.clients)

            # Add the requester to the list of clients
            new_client = None
            with db.transaction():
                new_client = Client.create(
                    internal_name=client.Id(),
                    ip=client.Ip(),
                    room=room.id,
                    port=client.Port())

            # Notify existing clients of the newly joined client.
            channel = room.guid.encode('utf-8')
            publish_offset = connect_api.response(pub_builder, new_client)

    if channel is not None:
        connect_offset = build_command(
            pub_builder,
            registrar.Registrar.Message.Message().Connect,
            publish_offset)
        pub_builder.Finish(connect_offset)
        publish_message = pub_builder.Output()
        pub_socket.send_multipart([channel, publish_message])

    req_offset = build_command(
        req_builder,
        message_type,
        message_offset)

    req_builder.Finish(req_offset)
    response = req_builder.Output()
    rep_socket.send(response)

    print('SERVER: Sent a response')
