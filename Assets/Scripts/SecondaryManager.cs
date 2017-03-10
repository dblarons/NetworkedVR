using UnityEngine;
using NetworkingFBS;
using FlatBuffers;
using Lidgren.Network;

namespace Assets.Scripts {
  public class SecondaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;
    UDPClient udpClient;
    public LocalObjectStore store;
    UpdateBuffer<Action> updateBuffer;

    float UPDATE_RATE = 0.033F;
    float nextUpdate = 0.0F;
    UpdateLerp<Action> updateLerp;

    bool isInitialized;

    void Start() {
      udpClient = new UDPClient();
      udpClient.Connect();

      updateBuffer = new UpdateBuffer<Action>();
      updateLerp = null;
    }

    void Update() {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        var buffer = new ByteBuffer(bytes);
        var action = Action.GetRootAsAction(buffer);
        updateBuffer.Enqueue(action);
      }

      if (nextUpdate < Time.time) {
        UpdateLerp<Action> update = updateBuffer.Dequeue();
        if (update != null) {
          updateLerp = update;
          var nextPosition = update.next.Position;
          logger.Log("LOG (client): Received position from client: " + nextPosition.X + " " + nextPosition.Y + " " + nextPosition.Z);
        }
      }

      if (updateLerp != null) {
        var lastPositionFB = updateLerp.last.Position;
        var nextPositionFB = updateLerp.next.Position;
        Vector3 lastPosition = new Vector3(lastPositionFB.X, lastPositionFB.Y, lastPositionFB.Z);
        Vector3 nextPosition = new Vector3(nextPositionFB.X, nextPositionFB.Y, nextPositionFB.Z);
        store.GetCube().transform.position = Vector3.Lerp(lastPosition, nextPosition, (nextUpdate - Time.time) / UPDATE_RATE);
        logger.Log("LOG (client): Lerped object position on Client");
      }
    }
  }
}
