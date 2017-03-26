using UnityEngine;
using NetworkingFBS;
using Assets.Scripts.Buffering;

namespace Assets.Scripts {
  public class SecondaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public LocalObjectStore localObjectStore;

    UDPClient udpClient;
    StateBuffer<FlatWorldState> stateBuffer;
    StateTransition<FlatWorldState> worldTransition;

    float UPDATE_RATE = 0.033F;
    float nextUpdateTime = 0.0F;

    void Start() {
      udpClient = new UDPClient();
      udpClient.Connect();

      stateBuffer = new StateBuffer<FlatWorldState>();
      worldTransition = null;
    }

    void Update() {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        stateBuffer.Enqueue(Serializer.BytesToFlatWorldState(bytes));
      }

      // When a lerping time is up, we get the newest position to lerp towards.
      if (nextUpdateTime < Time.time) {
        StateTransition<FlatWorldState> update = stateBuffer.Dequeue();
        if (update != null) {
          worldTransition = update;
        }
      }

      if (worldTransition != null) {
        if (worldTransition.last.PrimariesLength != worldTransition.next.PrimariesLength) {
          logger.LogError("PRIMARIES MISMATCH", "Primaries length did not match");
        }

        for (var i = 0; i < worldTransition.last.PrimariesLength; i++) {
          var lastPrimary = worldTransition.last.GetPrimaries(i);
          var nextPrimary = worldTransition.next.GetPrimaries(i);

          if (!lastPrimary.Guid.Equals(nextPrimary.Guid)) {
            // TODO(dblarons): See if this case happens only when new objects are created.
            logger.LogError("GUID MISMATCH", "GUIDs did not match during primary lerping");
          }

          var secondaryObject = localObjectStore.GetSecondary(nextPrimary.Guid);
          if (secondaryObject == null) {
            logger.Log("New object was created");

            // Secondary copy does not exist yet. Create one and register it.
            var prefabId = (PrefabId)nextPrimary.PrefabId;
            Vector3 position = Serializer.DeserializeVector3(nextPrimary.Position);
            Quaternion rotation = Serializer.DeserializeQuaternion(nextPrimary.Rotation);
            localObjectStore.Instantiate(prefabId, position, rotation, nextPrimary.Guid);
          } else {
            // Secondary copy exists. Lerp it.
            var objectLerp = new StateTransition<FlatNetworkedObject>(lastPrimary, nextPrimary);
            secondaryObject.Lerp(objectLerp.last, objectLerp.next, (nextUpdateTime - Time.time) / UPDATE_RATE);
          }
        }
      }
    }
  }
}
