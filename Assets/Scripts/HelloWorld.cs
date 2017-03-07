using UnityEngine;
using HW;
using FlatBuffers;
using Assets.Scripts;

public class HelloWorld : MonoBehaviour {
  UDPClient udpClient;
  UDPServer udpServer;
  public LocalObjectStore store;

  bool isInitialized;

  // Use this for initialization
  void Start () {
    isInitialized = false;
  }

  void Update () {
    if (!isInitialized) {
      if (Input.GetKeyDown(KeyCode.S)) {
        Debug.Log("LOG: Got key down S");
        // Spawn server.
        udpServer = new UDPServer();
        isInitialized = true;
      } else if (Input.GetKeyDown(KeyCode.C)) {
        Debug.Log("LOG: Got key down C");
        // Spawn client.
        udpClient = new UDPClient();
        udpClient.Connect();
        isInitialized = true;
      }
    }

    if (udpServer != null) {
      var cubePosition = store.GetCube().transform.position;

      Debug.Log("LOG (server): Got the cube position: " + cubePosition.x + " " + cubePosition.y + " " + cubePosition.z);

      Debug.Log("LOG (server): Building the action");

      FlatBufferBuilder builder = new FlatBufferBuilder(1024);
      var actionName = builder.CreateString("my action");
      var position = Vec3.CreateVec3(builder, cubePosition.x, cubePosition.y, cubePosition.z);

      // Build action.
      Action.StartAction(builder);
      Action.AddPosition(builder, position);
      Action.AddName(builder, actionName);
      var action = Action.EndAction(builder);
      builder.Finish(action.Value);

      byte[] bytes = builder.SizedByteArray();

      Debug.Log("LOG (server): Sending the action");

      udpServer.SendMessage(bytes);
    }

    if (udpClient != null) {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        var buffer = new ByteBuffer(bytes);
        var action = Action.GetRootAsAction(buffer);
        var position = action.Position;

        Debug.Log("LOG (client): Received position from client: " + position.X + " " + position.Y + " " + position.Z);

        store.GetCube().transform.position = new Vector3(position.X, position.Y, position.Z);

        Debug.Log("LOG (client): Set object position on Client");
      }
    }
  }
}
