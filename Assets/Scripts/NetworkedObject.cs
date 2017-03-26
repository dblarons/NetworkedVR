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

    public Offset<FlatNetworkedObject> Serialize(FlatBufferBuilder builder) {
      var guidStr = builder.CreateString(guid);

      FlatNetworkedObject.StartFlatNetworkedObject(builder);
      FlatNetworkedObject.AddGuid(builder, guidStr);
      FlatNetworkedObject.AddPrefabId(builder, (int)prefabId);
      FlatNetworkedObject.AddPosition(builder, Serializer.SerializeVector3(builder, position));
      FlatNetworkedObject.AddRotation(builder, Serializer.SerializeQuaternion(builder, rotation));
      return FlatNetworkedObject.EndFlatNetworkedObject(builder);
    }

    public void Lerp(FlatNetworkedObject last, FlatNetworkedObject next, float t) {
      Lerp(last.Position, next.Position, t);
      Lerp(last.Rotation, next.Rotation, t);
    }

    public void Lerp(FlatVector3 last, FlatVector3 next, float t) {
      Vector3 from = Serializer.DeserializeVector3(last);
      Vector3 to = Serializer.DeserializeVector3(next);
      transform.position = Vector3.Lerp(from, to, t);
    }

    public void Lerp(FlatQuaternion last, FlatQuaternion next, float t) {
      Quaternion from = Serializer.DeserializeQuaternion(last);
      Quaternion to = Serializer.DeserializeQuaternion(next);
      transform.rotation = Quaternion.Lerp(from, to, t);
    }
  }
}
