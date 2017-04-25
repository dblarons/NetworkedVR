import flatbuffers
import zmq

import registrar.Registrar.Command
import registrar.Registrar.Connect
import registrar.Registrar.Create
import registrar.Registrar.Message

from .utils import send_request, serialize_mock_client, build_command

SUB_PORT = '5555'
REQ_PORT = '5556'

context = zmq.Context()

print('Connecting to server...')
req_socket = context.socket(zmq.REQ)
req_socket.connect("tcp://localhost:%s" % REQ_PORT)

sub_socket = context.socket(zmq.SUB)
sub_socket.connect('tcp://localhost:%s' % SUB_PORT)

def build_create_request(builder):
    client = serialize_mock_client(builder)

    name = builder.CreateString('New room')
    registrar.Registrar.Create.CreateStart(builder)
    registrar.Registrar.Create.CreateAddName(builder, name)
    registrar.Registrar.Create.CreateAddClient(builder, client)
    return registrar.Registrar.Create.CreateEnd(builder)

def read_create_response(command):
    union_create = registrar.Registrar.Create.Create()
    union_create.Init(command.Message().Bytes, command.Message().Pos)

    room = union_create.Room()
    print('CLIENT: Created room with guid ' + str(room.Guid()))
    return room.Guid()

def read_connect_response(command):
    union_connect = registrar.Registrar.Connect.Connect()
    union_connect.Init(command.Message().Bytes, command.Message().Pos)

    client = union_connect.Client()
    print('CLIENT: Another client joined my room with ip ' + str(client.Ip()))

############ CREATE REQUEST ############

builder = flatbuffers.Builder(1024)
create_offset = build_create_request(builder)
offset = build_command(
    builder,
    registrar.Registrar.Message.Message().Create,
    create_offset)

print('CLIENT: Sending a Create request')
response = send_request(builder, req_socket, offset)

command = registrar.Registrar.Command.Command.GetRootAsCommand(response, 0)

if command.MessageType() == registrar.Registrar.Message.Message().Create:
    print('CLIENT: Received Create response from server')
    room_guid = read_create_response(command)
    sub_socket.setsockopt(zmq.SUBSCRIBE, room_guid)
else:
    print('ERROR: Expected Create message type but got another')

while True:
    [address, contents] = sub_socket.recv_multipart()
    connect_command = registrar.Registrar.Command.Command.GetRootAsCommand(contents, 0)
    read_connect_response(connect_command)

sub_socket.close()
context.term()
