#!/bin/bash

if [ -f /tmp/registrar/registrar.db ]; then
    rm /tmp/registrar/registrar.db
fi

if [ ! -d /tmp/registrar/ ]; then
    mkdir /tmp/registrar/
fi

python3 create.py

rm -rf registrar/Registrar

# Compile flatbuffers
flatc --python -o registrar/ flatbuffers/client.fbs
flatc --python -o registrar/ flatbuffers/command.fbs
flatc --python -o registrar/ flatbuffers/connect.fbs
flatc --python -o registrar/ flatbuffers/create.fbs
flatc --python -o registrar/ flatbuffers/join.fbs
flatc --python -o registrar/ flatbuffers/list.fbs
flatc --python -o registrar/ flatbuffers/room.fbs
