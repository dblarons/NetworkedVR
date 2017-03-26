using UnityEditor;

namespace Assets.Scripts {
  [CustomEditor(typeof(NetworkedObject))]
  class NetworkedObjectEditor : Editor {
    public override void OnInspectorGUI() {
      NetworkedObject myTarget = target;
      myTarget.prefabId = (PrefabId)EditorGUILayout.EnumPopup("Prefab ID", myTarget.prefabId);
    }
  }
}
