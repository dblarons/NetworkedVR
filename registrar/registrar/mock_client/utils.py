import registrar.Registrar.Client
import registrar.Registrar.Command
import registrar.Registrar.List

def send_request(builder, socket, offset):
    builder.Finish(offset)
    request = builder.Output()
    socket.send(request)
    response = socket.recv()
    return response

def serialize_mock_client(builder):
    client_id = builder.CreateString('New client')
    ip = builder.CreateString('129.0.0.1')
    registrar.Registrar.Client.ClientStart(builder)
    registrar.Registrar.Client.ClientAddId(builder, client_id)
    registrar.Registrar.Client.ClientAddIp(builder, ip)
    registrar.Registrar.Client.ClientAddPort(builder, 1234)
    return registrar.Registrar.Client.ClientEnd(builder)

def build_command(builder, message_type, message):
    registrar.Registrar.Command.CommandStart(builder)
    registrar.Registrar.Command.CommandAddMessageType(
        builder, message_type)
    registrar.Registrar.Command.CommandAddMessage(builder, message)
    return registrar.Registrar.Command.CommandEnd(builder)

def read_list_response(command):
    union_list = registrar.Registrar.List.List()
    union_list.Init(command.Message().Bytes, command.Message().Pos)

    rooms_length = union_list.RoomsLength()
    for i in range(rooms_length):
        room = union_list.Rooms(i)
        return room.Guid()
