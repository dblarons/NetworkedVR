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

    public static byte[] ToBytes(List<NetworkedObject> primaries, List<NetworkedObject> secondaries) {
      var builder = new FlatBufferBuilder(1024);

      var primariesOffsets = new Offset<ObjectState>[primaries.Count];
      for (var i = 0; i < primaries.Count; i++) {
         primariesOffsets[i] = primaries[i].Serialize(builder);
      }
      var primariesOffset = WorldUpdate.CreatePrimariesVector(builder, primariesOffsets);

      var secondariesOffsets = new Offset<ObjectState>[secondaries.Count];
      for (var i = 0; i < secondaries.Count; i++) {
         secondariesOffsets[i] = secondaries[i].Serialize(builder);
      }
      var secondariesOffset = WorldUpdate.CreatePrimariesVector(builder, primariesOffsets);

      WorldUpdate.StartWorldUpdate(builder);

      WorldUpdate.AddPrimaries(builder, primariesOffset);
      WorldUpdate.AddSecondaries(builder, secondariesOffset);

      var worldUpdate = WorldUpdate.EndWorldUpdate(builder);
      builder.Finish(worldUpdate.Value);
      return builder.SizedByteArray();
    }

    public static WorldUpdate FromBytes(byte[] bytes) {
      var buffer = new ByteBuffer(bytes);
      return WorldUpdate.GetRootAsWorldUpdate(buffer);
    }

    public static void Lerp(NetworkedObject obj, UpdateLerp<ObjectState> objectState, float t) {
      var fromPositionFB = objectState.last.Position;
      var toPositionFB = objectState.next.Position;
      Vector3 fromPosition = new Vector3(fromPositionFB.X, fromPositionFB.Y, fromPositionFB.Z);
      Vector3 toPosition = new Vector3(toPositionFB.X, toPositionFB.Y, toPositionFB.Z);

      var fromRotationFB = objectState.last.Rotation;
      var toRotationFB = objectState.next.Rotation;
      Quaternion fromRotation = new Quaternion(fromRotationFB.X, fromRotationFB.Y, fromRotationFB.Z, fromRotationFB.W);
      Quaternion toRotation = new Quaternion(toRotationFB.X, toRotationFB.Y, toRotationFB.Z, toRotationFB.W);

      obj.transform.position = Vector3.Lerp(fromPosition, toPosition, t);
      obj.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t);
    }
  }
}
