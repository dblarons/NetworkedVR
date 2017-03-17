using UnityEngine;
using NetworkingFBS;

namespace Assets.Scripts {
  public class SecondaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;
    UDPClient udpClient;
    public LocalObjectStore store;
    UpdateBuffer<Action> updateBuffer;
    UpdateLerp<Action> updateLerp;

    float UPDATE_RATE = 0.033F;
    float nextUpdate = 0.0F;

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
        updateBuffer.Enqueue(Serialization.FromBytes(bytes));
      }

      // When a lerping time is up, we get the newest position to lerp towards.
      if (nextUpdate < Time.time) {
        UpdateLerp<Action> update = updateBuffer.Dequeue();
        if (update != null) {
          updateLerp = update;
        }
      }

      if (updateLerp != null) {
        Serialization.Lerp(store.GetCube(), updateLerp, (nextUpdate - Time.time) / UPDATE_RATE);
      }
    }
  }
}
