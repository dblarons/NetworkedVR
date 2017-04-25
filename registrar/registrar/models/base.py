from peewee import Model

import registrar.config as config

class BaseModel(Model):
    class Meta:
        database = config.database
