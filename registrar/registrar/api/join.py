import registrar.Registrar.Join

def response(builder, room, clients):
    client_offsets = []
    for client in clients:
        offset = client.serialize(builder)
        client_offsets.append(offset)

    room_offset = room.serialize(builder)

    registrar.Registrar.Join.JoinStartClientsVector(builder, len(client_offsets))
    for offset in client_offsets:
        builder.PrependUOffsetTRelative(offset)
    client_vector_offset = builder.EndVector(len(client_offsets))

    registrar.Registrar.Join.JoinStart(builder)
    registrar.Registrar.Join.JoinAddRoom(builder, room_offset)
    registrar.Registrar.Join.JoinAddClients(builder, client_vector_offset)
    return registrar.Registrar.Join.JoinEnd(builder)
