from peewee import *

import registrar.Registrar.Client

from .base import BaseModel
from .room import Room

class Client(BaseModel):
    internal_name = CharField()
    ip = CharField()
    room = ForeignKeyField(Room, related_name='clients')
    port = IntegerField(default=0)

    def serialize(self, builder):
        client_id = builder.CreateString(self.internal_name)
        ip = builder.CreateString(self.ip)
        registrar.Registrar.Client.ClientStart(builder)
        registrar.Registrar.Client.ClientAddId(builder, client_id)
        registrar.Registrar.Client.ClientAddIp(builder, ip)
        registrar.Registrar.Client.ClientAddPort(builder, self.port)
        return registrar.Registrar.Client.ClientEnd(builder)
