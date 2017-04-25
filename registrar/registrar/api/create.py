import registrar.Registrar.Create

def response(builder, room):
    room_offset = room.serialize(builder)

    registrar.Registrar.Create.CreateStart(builder)
    registrar.Registrar.Create.CreateAddRoom(builder, room_offset)
    return registrar.Registrar.Create.CreateEnd(builder)
