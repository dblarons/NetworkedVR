using UnityEngine;
using HW;
using FlatBuffers;
using Lidgren.Network;
using System.Collections.Generic;

public class HelloWorld : MonoBehaviour {
  FlatBufferBuilder builder;

  static NetServer s_server;
  static NetClient s_client;

  // Use this for initialization
  void Start () {
    // Server
    NetPeerConfiguration serverConfig = new NetPeerConfiguration("chat");
    serverConfig.MaximumConnections = 100;
    serverConfig.Port = 14242;
    s_server = new NetServer(serverConfig);
    s_server.Start();

    // Client
    NetPeerConfiguration clientConfig = new NetPeerConfiguration("chat");
		clientConfig.AutoFlushSendQueue = false;
		s_client = new NetClient(clientConfig);
    s_client.Start();

		NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
		s_client.Connect("localhost", 14242, hail);
  }

  // Update is called once per frame
  void Update () {
    NetIncomingMessage im;
    while ((im = s_server.ReadMessage()) != null) {
      Debug.Log("Message received");
      // Handle incoming message.
      switch (im.MessageType) {
        case NetIncomingMessageType.DebugMessage:
        case NetIncomingMessageType.ErrorMessage:
        case NetIncomingMessageType.WarningMessage:
        case NetIncomingMessageType.VerboseDebugMessage:
          string text = im.ReadString();
          Debug.Log(text);
          break;

        case NetIncomingMessageType.StatusChanged:
          NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

          string reason = im.ReadString();
          Debug.Log(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

          if (status == NetConnectionStatus.Connected) {
            Debug.Log("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
          }
          break;

        case NetIncomingMessageType.Data:
          // incoming chat message from a client
          string chat = im.ReadString();

          Debug.Log("Broadcasting '" + chat + "'");

          // broadcast this to all connections, except sender
          List<NetConnection> all = s_server.Connections; // get copy

          if (all.Count > 0) {
            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
            s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
          }
          break;

        default:
          Debug.Log("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
          break;
      }

      s_server.Recycle(im);
    }

    var clientText = "foo bar";
    NetOutgoingMessage clientMsg = s_client.CreateMessage(clientText);
    s_client.SendMessage(clientMsg, NetDeliveryMethod.ReliableOrdered);
    Debug.Log("Sending '" + clientText + "'");
    s_client.FlushSendQueue();

    // var sendText = "foo bar";
    // NetOutgoingMessage clientMsg = s_client.CreateMessage(sendText);
		// s_client.SendMessage(clientMsg, NetDeliveryMethod.ReliableOrdered);
		// Debug.Log("Sending '" + sendText + "'");
		// s_client.FlushSendQueue();

    // {
    //   var actionName = builder.CreateString("my action");
    //   var position = Vec3.CreateVec3(builder, 1.0f, 2.0f, 3.0f);

    //   // Build action.
    //   Action.StartAction(builder);
    //   Action.AddPosition(builder, position);
    //   Action.AddName(builder, actionName);
    //   var action = Action.EndAction(builder);
    //   builder.Finish(action.Value);
    // }

    // {
    //   // Access flatbuffer fields.
    //   var action = Action.GetRootAsAction(buf);
    //   var position = action.Position;
    //   Debug.Log("From Client: " + action.Name);
    //   Debug.Log("From Client: " + position.X);
    //   Debug.Log("From Client: " + position.Y);
    //   Debug.Log("From Client: " + position.Z);
    // }
  }
}
