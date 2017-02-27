using UnityEngine;
using HW;
using FlatBuffers;
using Lidgren.Network;
using System.Collections.Generic;
using Assets.Scripts;

public class HelloWorld : MonoBehaviour {
  UDPClient udpClient;
  UDPServer udpServer;

  // Use this for initialization
  void Start () {
    udpClient = new UDPClient();
    udpServer = new UDPServer();

    udpClient.Connect();
  }

  void Update () {
    {
      byte[] bytes = udpServer.Read();
      if (bytes != null) {
        var buffer = new ByteBuffer(bytes);
        var action = Action.GetRootAsAction(buffer);
        var position = action.Position;
        Debug.Log("From Client: " + action.Name);
        Debug.Log("From Client: " + position.X);
        Debug.Log("From Client: " + position.Y);
        Debug.Log("From Client: " + position.Z);
      }
    }

    {
      FlatBufferBuilder builder = new FlatBufferBuilder(1024);
      var actionName = builder.CreateString("my action");
      var position = Vec3.CreateVec3(builder, 1.0f, 2.0f, 3.0f);

      // Build action.
      Action.StartAction(builder);
      Action.AddPosition(builder, position);
      Action.AddName(builder, actionName);
      var action = Action.EndAction(builder);
      builder.Finish(action.Value);

      byte[] bytes = builder.SizedByteArray();

      udpClient.SendMessage(bytes);
    }
  }
}
