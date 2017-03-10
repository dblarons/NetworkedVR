using UnityEngine;
using NetworkingFBS;
using FlatBuffers;

namespace Assets.Scripts {
  public class PrimaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;
    UDPServer udpServer;
    public LocalObjectStore store;

    float SEND_RATE = 0.033F;
    float nextSend = 0.0F;

    bool isInitialized;

    void Start() {
      udpServer = new UDPServer();
    }

    void Update() {
      if (nextSend < Time.time) {
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
    }
  }
}
