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

    NetworkedObject GetObject(Dictionary<string, NetworkedObject> lookup, string guid) {
      NetworkedObject obj = null;
      lookup.TryGetValue(guid, out obj);
      if (obj == null) {
        return null;
      }
      return obj;
    }

    public NetworkedObject GetSecondary(string guid) {
      return GetObject(secondaryLookup, guid);
    }

    public NetworkedObject GetPrimary(string guid) {
      return GetObject(primaryLookup, guid);
    }

    List<NetworkedObject> GetPrimaries() {
      return primaryLookup.Values.ToList();
    }

    public List<NetworkedObject> GetSecondaries() {
      return secondaryLookup.Values.ToList();
    }

    public Offset<FlatWorldState> SerializePrimaries(FlatBufferBuilder builder, float timestamp) {
      return Serialize(
        builder,
        GetPrimaries().Where(primary => primary.HasUpdate()).ToList(),
        new List<NetworkedObject>(),
        timestamp
      );
    }

    public Offset<FlatWorldState> SerializeSecondaries(FlatBufferBuilder builder,
        List<NetworkedObject> secondaries, float timestamp) {
      return Serialize(
        builder,
        new List<NetworkedObject>(),
        secondaries,
        timestamp
      );
    }

    public Offset<FlatWorldState> Serialize(FlatBufferBuilder builder, List<NetworkedObject>
        primaries, List<NetworkedObject> secondaries, float timestamp) {
      var primariesOffset = FlatWorldState.CreatePrimariesVector(
        builder,
        Serializer.SerializeNetworkedObjects(builder, primaries, timestamp)
      );

      var secondariesOffset = FlatWorldState.CreatePrimariesVector(
        builder,
        Serializer.SerializeNetworkedObjects(builder, secondaries, timestamp)
      );

      FlatWorldState.StartFlatWorldState(builder);

      FlatWorldState.AddPrimaries(builder, primariesOffset);
      FlatWorldState.AddSecondaries(builder, secondariesOffset);

      return FlatWorldState.EndFlatWorldState(builder);
    }
  }
}
