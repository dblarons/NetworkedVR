using HW;
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

    public void Connect(string host = "localhost", int port = 14242) {
		  NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
		  s_client.Connect(host, port, hail);
    }

    public void SendMessage(byte[] bytes) {
      NetOutgoingMessage message = s_client.CreateMessage(bytes.Length);
      message.Write(bytes);
      s_client.SendMessage(message, NetDeliveryMethod.UnreliableSequenced);
      s_client.FlushSendQueue();
    }
  }
}
