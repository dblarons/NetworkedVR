using FlatBuffers;
using NetMQ;
using NetMQ.Sockets;
using Registrar;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
  class RegistrarClient : MonoBehaviour {
    static ILogger logger = Debug.logger;

    readonly string ADDRESS = ">tcp://192.168.1.6:5556";

    public RegistrarClient() {
      AsyncIO.ForceDotNet.Force();
    }

    public List<Room> List() {
      FlatBufferBuilder builder = new FlatBufferBuilder(1024);
      var messageOffset = BuildCommand(builder, Message.List);
      builder.Finish(messageOffset.Value);
      byte[] buf = builder.SizedByteArray();

      byte[] response = null;
      using (var client = new RequestSocket(ADDRESS)) {
        client.SendFrame(buf);

        response = client.ReceiveFrameBytes();
      }

      var command = Command.GetRootAsCommand(new ByteBuffer(response));
      var unionList = command.GetMessage(new List());

      var rooms = new List<Room>();
      for (int i = 0; i < unionList.RoomsLength; i++) {
        rooms.Add(unionList.GetRooms(i));
      }
      return rooms;
    }

    public List<Client> Join(string guid, string clientId, string clientIp, int clientPort) {
      FlatBufferBuilder builder = new FlatBufferBuilder(1024);

      // Create join message.
      var guidOffset = builder.CreateString(guid);
      var clientOffset = BuildClient(builder, clientId, clientIp, clientPort);
      Registrar.Join.StartJoin(builder);
      Registrar.Join.AddGuid(builder, guidOffset);
      Registrar.Join.AddClient(builder, clientOffset);
      var joinOffset = Registrar.Join.EndJoin(builder);

      var commandOffset = BuildCommand(builder, Message.Join, joinOffset.Value);
      builder.Finish(commandOffset.Value);
      byte[] buf = builder.SizedByteArray();

      byte[] response = null;
      using (var client = new RequestSocket(ADDRESS)) {
        client.SendFrame(buf);

        response = client.ReceiveFrameBytes();
      }

      var command = Command.GetRootAsCommand(new ByteBuffer(response));
      var unionJoin = command.GetMessage(new Join());

      var clients = new List<Client>();
      for (int i = 0; i < unionJoin.ClientsLength; i++) {
        clients.Add(unionJoin.GetClients(i));
      }
      return clients;
    }

    Offset<Command> BuildCommand(FlatBufferBuilder builder, Message messageType, 
        int messageOffset = 0) {
      Command.StartCommand(builder);
      Command.AddMessageType(builder, messageType);
      Command.AddMessage(builder, messageOffset);
      return Command.EndCommand(builder);
    }

    Offset<Client> BuildClient(FlatBufferBuilder builder, string clientId, string clientIp, int clientPort) {
      var idOffset = builder.CreateString(clientId);
      var ipOffset = builder.CreateString(clientIp);
      Client.StartClient(builder);
      Client.AddId(builder, idOffset);
      Client.AddIp(builder, ipOffset);
      Client.AddPort(builder, clientPort);
      return Client.EndClient(builder);
    }
  }
}
