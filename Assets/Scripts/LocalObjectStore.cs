using UnityEngine;

namespace Assets.Scripts {
  public class LocalObjectStore : MonoBehaviour {
    public GameObject cube;
    public GameObject sphere;

    void Start() {
      // TODO(dblarons): Store objects, keyed by GUID.
    }

    public GameObject GetCube() {
      return cube;
    }

    public GameObject GetSphere() {
      return sphere;
    }
  }
}
