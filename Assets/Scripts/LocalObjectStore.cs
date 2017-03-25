using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts {
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

    public string RegisterPrimary(NetworkedObject obj) {
      string guid = Guid.NewGuid().ToString();
      primaryIds.Add(guid);
      primaryLookup.Add(guid, obj);
      return guid;
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
