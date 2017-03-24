using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
  public enum PrefabId {
    CUBE = 0,
    SPHERE,
  }

  public class PrefabLibrary : MonoBehaviour {
    public GameObject cube;
    public GameObject sphere;
    public Dictionary<PrefabId, GameObject> lookup;

    void Start() {
      lookup = new Dictionary<PrefabId, GameObject>();
      lookup.Add(PrefabId.CUBE, cube);
      lookup.Add(PrefabId.SPHERE, sphere);
    }
  }
}
