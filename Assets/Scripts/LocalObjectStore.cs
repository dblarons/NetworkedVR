using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts {
  public class NetworkedObject {
    public GameObject obj { get; set; }
    public string guid { get; set; }
    public PrefabId prefabId { get; set; }

    public NetworkedObject(GameObject obj, string guid, PrefabId prefabId) {
      this.obj = obj;
      this.guid = guid;
      this.prefabId = prefabId;
    }
  }

  public class LocalObjectStore : MonoBehaviour {
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

    public string RegisterPrimary(GameObject obj, PrefabId prefabId) {
      string guid = Guid.NewGuid().ToString();
      primaryIds.Add(guid);
      primaryLookup.Add(guid, new NetworkedObject(obj, guid, prefabId));
      return guid;
    }

    public void RegisterSecondary(GameObject obj, string guid, PrefabId prefabId) {
      secondaryIds.Add(guid);
      secondaryLookup.Add(guid, new NetworkedObject(obj, guid, prefabId));
    }

    public GameObject GetSecondary(string guid) {
      return secondaryLookup[guid].obj;
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
