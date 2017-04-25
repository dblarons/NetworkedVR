#!/bin/bash

if [ -f ./registrar.db ]; then
    rm registrar.db
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
