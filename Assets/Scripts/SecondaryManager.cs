using UnityEngine;
using NetworkingFBS;

namespace Assets.Scripts {
  public class SecondaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;
    UDPClient udpClient;
    public LocalObjectStore store;
    UpdateBuffer<Action> updateBuffer;

    float UPDATE_RATE = 0.033F;
    float nextUpdate = 0.0F;

    bool isInitialized;

    void Start() {
      udpClient = new UDPClient();
      udpClient.Connect();

      updateBuffer = new UpdateBuffer<Action>();
    }

    void Update() {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        updateBuffer.Enqueue(Serialization.FromBytes(bytes));
      }

      if (nextUpdate < Time.time) {
        UpdateLerp<Action> update = updateBuffer.Dequeue();
        if (update != null) {
          Serialization.Lerp(store.GetCube(), update, (nextUpdate - Time.time) / UPDATE_RATE);
        }
      }
    }
  }
}
