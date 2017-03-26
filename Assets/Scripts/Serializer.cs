using FlatBuffers;
using NetworkingFBS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
  public class Serializer : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public static Offset<FlatVector3> SerializeVector3(FlatBufferBuilder builder, Vector3 vector) {
      return FlatVector3.CreateFlatVector3(builder, vector.x, vector.y, vector.z);
    }

    public static Vector3 DeserializeVector3(FlatVector3 flatVector) {
      return new Vector3(flatVector.X, flatVector.Y, flatVector.Z);
    }

    public static Offset<FlatQuaternion> SerializeQuaternion(FlatBufferBuilder builder, Quaternion quaternion) {
      return FlatQuaternion.CreateFlatQuaternion(builder, quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    public static Quaternion DeserializeQuaternion(FlatQuaternion flatQuaternion) {
      return new Quaternion(flatQuaternion.X, flatQuaternion.Y, flatQuaternion.Z, flatQuaternion.W);
    }

    public static Offset<FlatNetworkedObject>[] SerializeNetworkedObjects(FlatBufferBuilder builder, List<NetworkedObject> objs) {
      var offsets = new Offset<FlatNetworkedObject>[objs.Count];
      for (var i = 0; i < objs.Count; i++) {
         offsets[i] = objs[i].Serialize(builder);
      }
      return offsets;
    }

    public static FlatWorldState FromBytes(byte[] bytes) {
      var buffer = new ByteBuffer(bytes);
      return FlatWorldState.GetRootAsFlatWorldState(buffer);
    }
  }
}
