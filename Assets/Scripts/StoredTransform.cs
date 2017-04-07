using UnityEngine;

namespace Assets.Scripts {
  public class StoredTransform {
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }

    public StoredTransform(Vector3 position, Quaternion rotation) {
      this.position = position;
      this.rotation = rotation;
    }

    public bool IsEqual(StoredTransform other) {
      if (Vector3.Distance(position, other.position) < 0.01 ||
          Quaternion.Angle(rotation, other.rotation) < 0.01) {
        return true;
      }
      return false;
    }
  }
}
