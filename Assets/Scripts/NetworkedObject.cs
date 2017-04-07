using Assets.Scripts.Buffering;
using FlatBuffers;
using NetworkingFBS;
using UnityEngine;

namespace Assets.Scripts {
  public class NetworkedObject : MonoBehaviour {
    public Transform transform {
      get { return GetComponent<Transform>(); }
    }

    Rigidbody rigidbody {
      get { return GetComponent<Rigidbody>(); }
    }
    
    public Vector3 position {
      get { return transform.position; }
    }

    public Quaternion rotation {
      get { return transform.rotation; }
    }

    Vector3 velocity {
      get { return rigidbody.velocity; }
    }

    public StoredTransform storedTransform;

    [HideInInspector]
    public string guid { get; set; }

    public PrefabId prefabId;

    bool forceUpdate;

    public StateBuffer<FlatNetworkedObject> buffer;

    public bool isInitialized { get; private set; }

    void Start() {
      // Always send an update when the object is first initialized.
      forceUpdate = true;
      buffer = new StateBuffer<FlatNetworkedObject>();
      isInitialized = true;
    }

    public bool HasUpdate() {
      if (forceUpdate) {
        forceUpdate = false;
        return true;
      }

      return velocity.magnitude > 0.001;
    }

    public Offset<FlatNetworkedObject> Serialize(FlatBufferBuilder builder, float timestamp) {
      var guidStr = builder.CreateString(guid);

      FlatNetworkedObject.StartFlatNetworkedObject(builder);
      FlatNetworkedObject.AddGuid(builder, guidStr);
      FlatNetworkedObject.AddPrefabId(builder, (int)prefabId);
      FlatNetworkedObject.AddTimestamp(builder, timestamp);
      FlatNetworkedObject.AddPosition(builder, Serializer.SerializeVector3(builder, position));
      FlatNetworkedObject.AddRotation(builder, Serializer.SerializeQuaternion(builder, rotation));
      FlatNetworkedObject.AddVelocity(builder, Serializer.SerializeVector3(builder, velocity));
      return FlatNetworkedObject.EndFlatNetworkedObject(builder);
    }

    /// <summary>
    /// Extrapolate this object's position based on the received object's velocity, position,
    /// and the amount of game time since the message was sent. Also set the rotation and velocity.
    /// </summary>
    /// <param name="received"></param>
    /// <param name="timeDelta">Elapsed time since the object was sent</param>
    public void Extrapolate(FlatNetworkedObject received, float timeDelta) {
      rigidbody.velocity = Serializer.DeserializeVector3(received.Velocity);
      var receivedPosition = Serializer.DeserializeVector3(received.Position);
      transform.position = receivedPosition + velocity * timeDelta;
      transform.rotation = Serializer.DeserializeQuaternion(received.Rotation);
    }

    public void Lerp(StateTransition<FlatNetworkedObject> transition, float t) {
      Lerp(transition.last, transition.next, t);
    }

    public void Lerp(FlatNetworkedObject last, FlatNetworkedObject next, float t) {
      var lerpedPosition = LerpVector3(last.Position, next.Position, t);
      var lerpedRotation = LerpQuaternion(last.Rotation, next.Rotation, t);
      storedTransform = new StoredTransform(lerpedPosition, lerpedRotation);
    }

    public Vector3 LerpVector3(FlatVector3 last, FlatVector3 next, float t) {
      var from = Serializer.DeserializeVector3(last);
      var to = Serializer.DeserializeVector3(next);
      var lerpedPosition = Vector3.Lerp(from, to, t);
      transform.position = lerpedPosition;
      return lerpedPosition;
    }

    public Quaternion LerpQuaternion(FlatQuaternion last, FlatQuaternion next, float t) {
      var from = Serializer.DeserializeQuaternion(last);
      var to = Serializer.DeserializeQuaternion(next);
      var lerpedRotation = Quaternion.Lerp(from, to, t);
      transform.rotation = lerpedRotation;
      return lerpedRotation;
    }

    public bool HasMoved() {
      if (storedTransform != null) {
        return !storedTransform.IsEqual(new StoredTransform(position, rotation));
      }
      return false;
    }
  }
}
