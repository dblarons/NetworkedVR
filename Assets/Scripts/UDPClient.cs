using Lidgren.Network;
using UnityEngine;

namespace Assets.Scripts {
  class UDPClient {
    NetClient s_client;

    public UDPClient() {
      NetPeerConfiguration clientConfig = new NetPeerConfiguration("chat");
		  clientConfig.AutoFlushSendQueue = false;
		  s_client = new NetClient(clientConfig);
      s_client.Start();
    }

    public void Connect(string host = "192.168.1.10", int port = 14242) {
		  NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
		  s_client.Connect(host, port, hail);
    }

    public byte[] Read() {
      NetIncomingMessage im = s_client.ReadMessage();
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

      s_client.Recycle(im);

      return bytes;
    }

    public void SendMessage(byte[] bytes) {
      NetOutgoingMessage message = s_client.CreateMessage(bytes.Length);
      message.Write(bytes);
      s_client.SendMessage(message, NetDeliveryMethod.UnreliableSequenced);
      s_client.FlushSendQueue();
    }

    public void Shutdown() {
      s_client.Shutdown("Application exiting");
    }
  }
}
