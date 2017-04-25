# networks-project
Final project for CS283 Networks

## Setup

`python3 -m venv ~/ENV/networks-project`
`source ~/ENV/networks-project/bin/activate`

## Configure database

`./setup.sh`

## Run server application

`python3 server_app.py`

## Run simple client application

First, start a client that creates a room:

`python3 creator_app.py`

Then, start a client that joins the room (can call it multiple times):

`python3 joiner_app.py`
