from peewee import *

import registrar.config as config
from registrar.models.client import Client
from registrar.models.room import Room

config.database.connect();
config.database.create_tables([Client, Room])
