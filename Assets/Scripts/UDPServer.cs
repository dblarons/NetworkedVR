using Lidgren.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
  class UDPServer {
    static readonly int MAX_CONNECTIONS = 100;

    NetServer s_server;

    public UDPServer(int port = 14242) {
      NetPeerConfiguration serverConfig = new NetPeerConfiguration("chat");
      serverConfig.MaximumConnections = MAX_CONNECTIONS;
      serverConfig.Port = port;
      s_server = new NetServer(serverConfig);
      s_server.Start();
    }

    public byte[] Read() {
      NetIncomingMessage im = s_server.ReadMessage();
      if (im == null) {
        return null;
      }

      byte[] bytes = null;
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
          bytes = im.ReadBytes(im.LengthBytes);
          break;

        default:
          Debug.Log("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
          break;
      }

      s_server.Recycle(im);

      return bytes;
    }

    public void SendMessage(byte[] bytes) {
      List<NetConnection> all = s_server.Connections;
      if (all.Count > 0) {
        Debug.Log("LOG: Server is sending the message to " + all.Count + " clients");

        NetOutgoingMessage om = s_server.CreateMessage();
        om.Write(bytes);
        s_server.SendMessage(om, all, NetDeliveryMethod.UnreliableSequenced, 0);
      }
    }
  }
}
