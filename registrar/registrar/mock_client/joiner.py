import flatbuffers
import sys
import zmq

import registrar.Registrar.Command
import registrar.Registrar.Join
import registrar.Registrar.Message

from .utils import send_request, serialize_mock_client, build_command, read_list_response

REQ_PORT = '5556'

context = zmq.Context()

print('Connecting to server...')
req_socket = context.socket(zmq.REQ)
req_socket.connect("tcp://localhost:%s" % REQ_PORT)

def build_join_request(builder, guid):
    guid = builder.CreateString(guid)
    client = serialize_mock_client(builder)

    registrar.Registrar.Join.JoinStart(builder)
    registrar.Registrar.Join.JoinAddGuid(builder, guid)
    registrar.Registrar.Join.JoinAddClient(builder, client)
    return registrar.Registrar.Join.JoinEnd(builder)

def read_join_response(command):
    union_join = registrar.Registrar.Join.Join()
    union_join.Init(command.Message().Bytes, command.Message().Pos)

    room = union_join.Room()
    print('CLIENT: Joined room with name ' + str(room.Name()))

    clients_length = union_join.ClientsLength()
    print('CLIENT: Joined room that has ' + str(clients_length) + ' other clients')
    for i in range(clients_length):
        client = union_join.Clients(i)
        print('CLIENT: Client ' + str(i) + ' in room has ip ' + str(client.Ip()))

############ LIST REQUEST ############

builder = flatbuffers.Builder(1024)
offset = build_command(
    builder,
    registrar.Registrar.Message.Message().List,
    0)

print('CLIENT: Sending a List request');
response = send_request(builder, req_socket, offset)

command = registrar.Registrar.Command.Command.GetRootAsCommand(response, 0)

first_room_guid = None
if command.MessageType() == registrar.Registrar.Message.Message().List:
    print('CLIENT: Received List response from server')
    first_room_guid = read_list_response(command)
else:
    print('ERROR: Expected List message type but got another')

############ JOIN REQUEST ############

if first_room_guid is None:
    print('ERROR: Client did not find any rooms')
    sys.exit()

print('CLIENT: Joining room with guid: ' + str(first_room_guid))

builder = flatbuffers.Builder(1024)
join_offset = build_join_request(builder, first_room_guid)
offset = build_command(
    builder,
    registrar.Registrar.Message.Message().Join,
    join_offset)

print('CLIENT: Sending a Join request')
response = send_request(builder, req_socket, offset)

command = registrar.Registrar.Command.Command.GetRootAsCommand(response, 0)

if command.MessageType() == registrar.Registrar.Message.Message().Join:
    read_join_response(command)
    print('CLIENT: Received Join response from server')
else:
    print('ERROR: Expected Join message type but got another')

req_socket.close()
context.term()
