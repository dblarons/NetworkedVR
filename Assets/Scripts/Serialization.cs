using FlatBuffers;
using NetworkingFBS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
  public class Serialization : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public static Offset<ObjectState> ToFlatbuffer(FlatBufferBuilder builder, NetworkedObject networkedObj) {
      var position = networkedObj.obj.transform.position;
      var rotation = networkedObj.obj.transform.rotation;

      var guid = builder.CreateString(networkedObj.guid);

      ObjectState.StartObjectState(builder);
      ObjectState.AddPosition(builder, Position.CreatePosition(builder, position.x, position.y, position.z));
      ObjectState.AddRotation(builder, Rotation.CreateRotation(builder, rotation.x, rotation.y, rotation.z, rotation.w));
      ObjectState.AddGuid(builder, guid);
      return ObjectState.EndObjectState(builder);
    }

    public static byte[] ToBytes(List<NetworkedObject> primaries, List<NetworkedObject> secondaries) {
      var builder = new FlatBufferBuilder(1024);

      WorldUpdate.StartPrimariesVector(builder, primaries.Count);
      foreach (var primary in primaries) {
        ToFlatbuffer(builder, primary);
      }
      var primariesOffset = builder.EndVector();

      WorldUpdate.StartSecondariesVector(builder, secondaries.Count);
      foreach (var secondary in secondaries) {
        ToFlatbuffer(builder, secondary);
      }
      var secondariesOffset = builder.EndVector();

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

    public static GameObject Instantiate(GameObject prefab, ObjectState objectState) {
      var positionFB = objectState.Position;
      var rotationFB = objectState.Rotation;

      Vector3 position = new Vector3(positionFB.X, positionFB.Y, positionFB.Z);
      Quaternion rotation = new Quaternion(rotationFB.X, rotationFB.Y, rotationFB.Z, rotationFB.W);

      return Instantiate(prefab, position, rotation);
    }

    public static void Lerp(GameObject obj, UpdateLerp<ObjectState> objectState, float t) {
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
