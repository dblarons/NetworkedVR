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
        logger.Log("LOG (server): Sending the action");
        udpServer.SendMessage(Serialization.ToBytes(store.GetCube()));
      }
    }
  }
}
