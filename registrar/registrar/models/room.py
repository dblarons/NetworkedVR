from peewee import *

import registrar.Registrar.Room

from .base import BaseModel

class Room(BaseModel):
    guid = CharField(unique=True)
    name = CharField()

    def serialize(self, builder):
        client_offsets = []
        for client in self.clients:
            offset = client.serialize(builder)
            client_offsets.append(offset)

        guid = builder.CreateString(self.guid)
        name = builder.CreateString(self.name)

        registrar.Registrar.Room.RoomStartClientsVector(builder, len(client_offsets))
        for offset in client_offsets:
            builder.PrependUOffsetTRelative(offset)
        clients = builder.EndVector(len(client_offsets))

        registrar.Registrar.Room.RoomStart(builder)
        registrar.Registrar.Room.RoomAddGuid(builder, guid)
        registrar.Registrar.Room.RoomAddName(builder, name)
        registrar.Registrar.Room.RoomAddClients(builder, clients)

        return registrar.Registrar.Room.RoomEnd(builder)
