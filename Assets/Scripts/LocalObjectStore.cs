using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using FlatBuffers;
using NetworkingFBS;

namespace Assets.Scripts {
  public class LocalObjectStore : MonoBehaviour {
    public PrefabLibrary prefabLibrary;

    Dictionary<string, NetworkedObject> primaryLookup;
    Dictionary<string, NetworkedObject> secondaryLookup;

    void Start() {
      primaryLookup = new Dictionary<string, NetworkedObject>();
      secondaryLookup = new Dictionary<string, NetworkedObject>();
    }

    /// <summary>
    /// Given a prefabId, look up the associated prefab, instantiate it, and register it in 
    /// the primary store, identified by its GUID.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Instantiate(PrefabId prefabId, Vector3 position, Quaternion rotation) {
      GameObject prefab = prefabLibrary.lookup[prefabId];
      GameObject obj = Instantiate(prefab, position, rotation);
      RegisterPrimary(obj.GetComponent<NetworkedObject>());
    }

    /// <summary>
    /// Given a prefabId, look up the associated prefab, instantiate it, and register it in
    /// the secondary store, identified by its GUID.
    /// </summary>
    /// <param name="prefabId"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="guid"></param>
    public void Instantiate(PrefabId prefabId, Vector3 position, Quaternion rotation, string guid) {
      GameObject prefab = prefabLibrary.lookup[prefabId];
      GameObject obj = Instantiate(prefab, position, rotation);
      RegisterSecondary(obj.GetComponent<NetworkedObject>(), guid);
    }

    /// <summary>
    /// Register a given object as a primary object and generate a GUID for it.
    /// </summary>
    /// <param name="obj"></param>
    public void RegisterPrimary(NetworkedObject obj) {
      string guid = Guid.NewGuid().ToString();
      obj.guid = guid;
      primaryLookup.Add(guid, obj);
    }

    /// <summary>
    /// Register an object as a secondary object and assign it a predefined GUID.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="guid"></param>
    public void RegisterSecondary(NetworkedObject obj, string guid) {
      secondaryLookup.Add(guid, obj);
      obj.guid = guid;
    }

    public NetworkedObject GetSecondary(string guid) {
      NetworkedObject secondaryObject = null;
      secondaryLookup.TryGetValue(guid, out secondaryObject);
      if (secondaryObject == null) {
        return null;
      }
      return secondaryObject;
    }

    List<NetworkedObject> GetPrimaries() {
      return primaryLookup.Values.ToList();
    }

    List<NetworkedObject> GetSecondaries() {
      return secondaryLookup.Values.ToList();
    }

    public Offset<FlatWorldState> Serialize(FlatBufferBuilder builder) {
      var primariesOffset = FlatWorldState.CreatePrimariesVector(
        builder,
        Serializer.SerializeNetworkedObjects(
          builder,
          GetPrimaries().Where(primary => primary.HasUpdate()).ToList()
        )
      );

      var secondariesOffset = FlatWorldState.CreatePrimariesVector(
        builder,
        Serializer.SerializeNetworkedObjects(
          builder,
          GetSecondaries().Where(secondary => secondary.HasUpdate()).ToList()
        )
      );

      FlatWorldState.StartFlatWorldState(builder);

      FlatWorldState.AddPrimaries(builder, primariesOffset);
      FlatWorldState.AddSecondaries(builder, secondariesOffset);

      return FlatWorldState.EndFlatWorldState(builder);
    }
  }
}
