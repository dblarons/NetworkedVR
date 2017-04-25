import registrar.Registrar.Connect

def response(builder, client):
    client_offset = client.serialize(builder)

    registrar.Registrar.Connect.ConnectStart(builder)
    registrar.Registrar.Connect.ConnectAddClient(builder, client_offset)
    return registrar.Registrar.Connect.ConnectEnd(builder)
