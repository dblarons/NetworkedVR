using UnityEngine;
using NetworkingFBS;
using FlatBuffers;

namespace Assets.Scripts {
  public class Networking : MonoBehaviour {
    static ILogger logger = Debug.logger;
    UDPClient udpClient;
    UDPServer udpServer;
    public LocalObjectStore store;

    float SEND_RATE = 0.033F;
    float nextSend = 0.0F;

    bool isInitialized;

    void Start() {
      isInitialized = false;
      logger.filterLogType = LogType.Error;
    }

    void Update() {
      if (!isInitialized) {
        if (Input.GetKeyDown(KeyCode.S)) {
          // Spawn server.
          udpServer = new UDPServer();
          isInitialized = true;
        } else if (Input.GetKeyDown(KeyCode.C)) {
          // Spawn client.
          udpClient = new UDPClient();
          udpClient.Connect();
          isInitialized = true;
        }
      }

      if (udpServer != null && Time.time > nextSend) {
        nextSend = Time.time + SEND_RATE;

        var cubePosition = store.GetCube().transform.position;

        logger.Log("LOG (server): Got the cube position: " + cubePosition.x + " " + cubePosition.y + " " + cubePosition.z);

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

        logger.Log("LOG (server): Sending the action");

        udpServer.SendMessage(bytes);
      }

      if (udpClient != null) {
        byte[] bytes = udpClient.Read();
        if (bytes != null) {
          var buffer = new ByteBuffer(bytes);
          var action = Action.GetRootAsAction(buffer);
          var position = action.Position;

          logger.Log("LOG (client): Received position from client: " + position.X + " " + position.Y + " " + position.Z);

          store.GetCube().transform.position = new Vector3(position.X, position.Y, position.Z);

          logger.Log("LOG (client): Set object position on Client");
        }
      }
    }
  }
}