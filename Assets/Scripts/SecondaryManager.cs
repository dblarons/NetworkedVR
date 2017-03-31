using UnityEngine;
using NetworkingFBS;
using Assets.Scripts.Buffering;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts {
  public class SecondaryTransition {
    public NetworkedObject obj;
    public StateTransition<FlatNetworkedObject> transition;

    public SecondaryTransition(NetworkedObject o, StateTransition<FlatNetworkedObject> t) {
      obj = o;
      transition = t;
    }
  }

  public class SecondaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public LocalObjectStore localObjectStore;

    UDPClient udpClient;
    StateBuffer<FlatWorldState> stateBuffer;
    StateTransition<FlatWorldState> worldTransition;
    List<SecondaryTransition> secondaryTransitions;

    float UPDATE_RATE = 0.033F;
    float nextUpdateTime = 0.0F;

    void Start() {
      udpClient = new UDPClient();
      udpClient.Connect();

      stateBuffer = new StateBuffer<FlatWorldState>();
      worldTransition = null;
      secondaryTransitions = new List<SecondaryTransition>();
    }

    void Update() {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        FlatWorldState worldState = Serializer.BytesToFlatWorldState(bytes);
        for (var i = 0; i < worldState.PrimariesLength; i++) {
          var receivedPrimary = worldState.GetPrimaries(i);
          var secondary = localObjectStore.GetSecondary(receivedPrimary.Guid);
          if (secondary == null) {
            // Secondary copy does not exist yet. Create one and register it.
            var prefabId = (PrefabId)receivedPrimary.PrefabId;
            localObjectStore.Instantiate(
              prefabId,
              Serializer.DeserializeVector3(receivedPrimary.Position),
              Serializer.DeserializeQuaternion(receivedPrimary.Rotation),
              receivedPrimary.Guid
            );
          } else {
            // Add state update to secondary object's StateBuffer.
            secondary.buffer.Enqueue(receivedPrimary);
          }
        }
      }

      // When a lerping time is up, we get the newest position to lerp towards.
      if (nextUpdateTime < Time.time) {
        secondaryTransitions = localObjectStore
            .GetSecondaries()
            .Where(sec => sec.isInitialized)
            .Select(sec => new SecondaryTransition(sec, sec.buffer.Dequeue()))
            .Where(trans => trans.transition != null).ToList();
      }

      foreach (var transition in secondaryTransitions) {
        transition.obj.Lerp(transition.transition, (nextUpdateTime - Time.time) / UPDATE_RATE);
      }
    }
  }
}
