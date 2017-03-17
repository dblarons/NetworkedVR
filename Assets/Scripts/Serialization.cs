using FlatBuffers;
using NetworkingFBS;
using UnityEngine;

namespace Assets.Scripts {
  public class Serialization {
    public static byte[] ToBytes(GameObject obj) {
      var position = obj.transform.position;
      var rotation = obj.transform.rotation;

      FlatBufferBuilder builder = new FlatBufferBuilder(1024);
      var guid = builder.CreateString("TODO: Put GUID here");

      Action.StartAction(builder);
      Action.AddPosition(builder, Position.CreatePosition(builder, position.x, position.y, position.z));
      Action.AddRotation(builder, Rotation.CreateRotation(builder, rotation.x, rotation.y, rotation.z, rotation.w));
      Action.AddGuid(builder, guid);
      var action = Action.EndAction(builder);
      builder.Finish(action.Value);

      return builder.SizedByteArray();
    }

    public static Action FromBytes(byte[] bytes) {
      var buffer = new ByteBuffer(bytes);
      return Action.GetRootAsAction(buffer);
    }

    public static void Lerp(GameObject obj, UpdateLerp<Action> action, float t) {
      var fromPositionFB = action.last.Position;
      var toPositionFB = action.next.Position;
      Vector3 fromPosition = new Vector3(fromPositionFB.X, fromPositionFB.Y, fromPositionFB.Z);
      Vector3 toPosition = new Vector3(toPositionFB.X, toPositionFB.Y, toPositionFB.Z);

      var fromRotationFB = action.last.Rotation;
      var toRotationFB = action.next.Rotation;
      Quaternion fromRotation = new Quaternion(fromRotationFB.X, fromRotationFB.Y, fromRotationFB.Z, fromRotationFB.W);
      Quaternion toRotation = new Quaternion(toRotationFB.X, toRotationFB.Y, toRotationFB.Z, toRotationFB.W);

      obj.transform.position = Vector3.Lerp(fromPosition, toPosition, t);
      obj.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t);
    }
  }
}
