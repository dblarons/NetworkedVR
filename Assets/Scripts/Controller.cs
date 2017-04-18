using UnityEngine;
using VRTK;

namespace Assets.Scripts {
  public class Controller : MonoBehaviour {
    public GameObject controller;
    public LocalObjectStore localObjectStore;

    void Start() {
      controller.GetComponent<VRTK_ControllerEvents>().TriggerPressed += DoTriggerPressed;
    }

    void DoTriggerPressed(object sender, ControllerInteractionEventArgs e) {
      localObjectStore.Instantiate(PrefabId.SPHERE, controller.transform.position, controller.transform.rotation);
    }
  }
}
