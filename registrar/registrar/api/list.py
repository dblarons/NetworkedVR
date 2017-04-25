import registrar.Registrar.List

def response(builder, rooms):
    room_offsets = []
    for room in rooms:
        offset = room.serialize(builder)
        room_offsets.append(offset)

    registrar.Registrar.List.ListStartRoomsVector(builder, len(room_offsets))
    for offset in room_offsets:
        builder.PrependUOffsetTRelative(offset)
    room_vector_offset = builder.EndVector(len(room_offsets))

    registrar.Registrar.List.ListStart(builder)
    registrar.Registrar.List.ListAddRooms(builder, room_vector_offset)
    return registrar.Registrar.List.ListEnd(builder)

