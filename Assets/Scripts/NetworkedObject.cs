using UnityEngine;
using UnityEditor;

namespace Assets.Scripts {
  public class NetworkedObject : MonoBehaviour {
    public Transform transform {
      get { return GetComponent<Transform>(); }
    }
    
    public Vector3 position {
      get { return transform.position; }
    }

    public Quaternion rotation {
      get { return transform.rotation; }
    }

    [HideInInspector]
    public string guid { get; set; }

    public PrefabId prefabId;
  }
}
