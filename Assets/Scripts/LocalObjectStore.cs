using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts {
  public class LocalObjectStore : MonoBehaviour {
    public PrefabLibrary prefabLibrary;

    List<string> primaryIds;
    List<string> secondaryIds;
    Dictionary<string, NetworkedObject> primaryLookup;
    Dictionary<string, NetworkedObject> secondaryLookup;

    void Start() {
      primaryIds = new List<string>();
      secondaryIds = new List<string>();

      primaryLookup = new Dictionary<string, NetworkedObject>();
      secondaryLookup = new Dictionary<string, NetworkedObject>();
    }

    /// <summary>
    /// Mirror the Unity `Instantiate` API, but register the object in the primary store when it 
    /// is instantiated.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
      GameObject obj = UnityEngine.Object.Instantiate(prefab, position, rotation);
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
      GameObject obj = UnityEngine.Object.Instantiate(prefab, position, rotation);
      RegisterSecondary(obj.GetComponent<NetworkedObject>(), guid);
    }

    public void RegisterPrimary(NetworkedObject obj) {
      string guid = Guid.NewGuid().ToString();
      primaryIds.Add(guid);
      primaryLookup.Add(guid, obj);
    }

    public void RegisterSecondary(NetworkedObject obj, string guid) {
      secondaryIds.Add(guid);
      secondaryLookup.Add(guid, obj);
    }

    public NetworkedObject GetSecondary(string guid) {
      NetworkedObject secondaryObject = null;
      secondaryLookup.TryGetValue(guid, out secondaryObject);
      if (secondaryObject == null) {
        return null;
      }
      return secondaryObject;
    }

    public List<NetworkedObject> GetPrimaries() {
      var primaries = new List<NetworkedObject>();
      foreach (var primaryId in primaryIds) {
        primaries.Add(primaryLookup[primaryId]);
      }
      return primaries;
    }

    public List<NetworkedObject> GetSecondaries() {
      var primaries = new List<NetworkedObject>();
      foreach (var secondaryId in secondaryIds) {
        primaries.Add(secondaryLookup[secondaryId]);
      }
      return primaries;
    }
  }
}
