using UnityEngine;
using NetworkingFBS;
using FlatBuffers;

namespace Assets.Scripts {
  public class PrimaryManager : MonoBehaviour {
    public LocalObjectStore localObjectStore;
    public PrefabLibrary prefabLibrary;

    static ILogger logger = Debug.logger;
    UDPServer udpServer;

    float SEND_RATE = 0.033F;
    float nextSend = 0.0F;

    bool isInitialized = false;

    void Start() {
      udpServer = new UDPServer();
    }

    void Update() {
      if (!isInitialized) {
        var cube = Instantiate(prefabLibrary.cube, new Vector3(-0.1f, 0.5f, 2.6f), Quaternion.identity);
        var sphere = Instantiate(prefabLibrary.sphere, new Vector3(0.1f, 0.5f, 2.6f), Quaternion.identity);
        localObjectStore.RegisterPrimary(cube, PrefabId.CUBE);
        localObjectStore.RegisterPrimary(sphere, PrefabId.SPHERE);

        isInitialized = true;
      }

      if (nextSend < Time.time) {
        nextSend = Time.time + SEND_RATE;
        logger.Log("LOG (server): Sending world update");

        udpServer.SendMessage(Serialization.ToBytes(localObjectStore.GetPrimaries(), localObjectStore.GetSecondaries()));
      }
    }
  }
}
