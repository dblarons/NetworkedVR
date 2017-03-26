using UnityEngine;
using NetworkingFBS;

namespace Assets.Scripts {
  public class SecondaryManager : MonoBehaviour {
    public LocalObjectStore localObjectStore;

    static ILogger logger = Debug.logger;
    UDPClient udpClient;
    UpdateBuffer<WorldUpdate> updateBuffer;
    UpdateLerp<WorldUpdate> worldLerp;

    float UPDATE_RATE = 0.033F;
    float nextUpdate = 0.0F;

    void Start() {
      udpClient = new UDPClient();
      udpClient.Connect();

      updateBuffer = new UpdateBuffer<WorldUpdate>();
      worldLerp = null;
    }

    void Update() {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        updateBuffer.Enqueue(Serializer.FromBytes(bytes));
      }

      // When a lerping time is up, we get the newest position to lerp towards.
      if (nextUpdate < Time.time) {
        UpdateLerp<WorldUpdate> update = updateBuffer.Dequeue();
        if (update != null) {
          worldLerp = update;
        }
      }

      if (worldLerp != null) {
        if (worldLerp.last.PrimariesLength != worldLerp.next.PrimariesLength) {
          logger.LogError("PRIMARIES MISMATCH", "Primaries length did not match");
        }

        for (var i = 0; i < worldLerp.last.PrimariesLength; i++) {
          var lastPrimary = worldLerp.last.GetPrimaries(i);
          var nextPrimary = worldLerp.next.GetPrimaries(i);

          if (!lastPrimary.Guid.Equals(nextPrimary.Guid)) {
            // TODO(dblarons): See if this case happens only when new objects are created.
            logger.LogError("GUID MISMATCH", "GUIDs did not match during primary lerping");
          }

          var secondaryObject = localObjectStore.GetSecondary(nextPrimary.Guid);
          if (secondaryObject == null) {
            logger.Log("New object was created");

            // Secondary copy does not exist yet. Create one and register it.
            var prefabId = (PrefabId)nextPrimary.PrefabId;
            Vector3 position = Serializer.DeserializePosition(nextPrimary.Position);
            Quaternion rotation = Serializer.DeserializeRotation(nextPrimary.Rotation);
            localObjectStore.Instantiate(prefabId, position, rotation, nextPrimary.Guid);
          } else {
            // Secondary copy exists. Lerp it.
            var objectLerp = new UpdateLerp<ObjectState>(lastPrimary, nextPrimary);
            Serializer.Lerp(secondaryObject, objectLerp, (nextUpdate - Time.time) / UPDATE_RATE);
          }
        }
      }
    }
  }
}
