using FlatBuffers;
using NetworkingFBS;
using UnityEngine;

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

    public Offset<ObjectState> Serialize(FlatBufferBuilder builder) {
      var guidStr = builder.CreateString(guid);

      ObjectState.StartObjectState(builder);
      ObjectState.AddGuid(builder, guidStr);
      ObjectState.AddPrefabId(builder, (int)prefabId);
      ObjectState.AddPosition(builder, Serializer.SerializePosition(builder, position));
      ObjectState.AddRotation(builder, Serializer.SerializeRotation(builder, rotation));
      return ObjectState.EndObjectState(builder);
    }
  }
}
