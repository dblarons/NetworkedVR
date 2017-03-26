using FlatBuffers;
using NetworkingFBS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
  public class Serializer : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public static Offset<Position> SerializePosition(FlatBufferBuilder builder, Vector3 position) {
      return Position.CreatePosition(builder, position.x, position.y, position.z);
    }

    public static Vector3 DeserializePosition(Position position) {
      return new Vector3(position.X, position.Y, position.Z);
    }

    public static Offset<Rotation> SerializeRotation(FlatBufferBuilder builder, Quaternion rotation) {
      return Rotation.CreateRotation(builder, rotation.x, rotation.y, rotation.z, rotation.w);
    }

    public static Quaternion DeserializeRotation(Rotation rotation) {
      return new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
    }

    public static Offset<ObjectState>[] SerializeNetworkedObjects(FlatBufferBuilder builder, List<NetworkedObject> objs) {
      var offsets = new Offset<ObjectState>[objs.Count];
      for (var i = 0; i < objs.Count; i++) {
         offsets[i] = objs[i].Serialize(builder);
      }
      return offsets;
    }

    public static WorldUpdate FromBytes(byte[] bytes) {
      var buffer = new ByteBuffer(bytes);
      return WorldUpdate.GetRootAsWorldUpdate(buffer);
    }
  }
}
