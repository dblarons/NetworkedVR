﻿using UnityEngine;
using VRTK;

namespace Assets.Scripts {
  public class PrimaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public GameObject controller;
    public LocalObjectStore localObjectStore;

    UDPServer udpServer;

    float SEND_RATE = 0.033F;
    float nextSend = 0.0F;
    bool isInitialized = false;

    void Start() {
      controller.GetComponent<VRTK_ControllerEvents>().TriggerPressed += DoTriggerPressed;
      udpServer = new UDPServer();
    }

    void Update() {
      if (!isInitialized) {
        localObjectStore.Instantiate(PrefabId.CUBE, new Vector3(-0.1f, 0.5f, 2.6f), Quaternion.identity);
        localObjectStore.Instantiate(PrefabId.SPHERE, new Vector3(0.1f, 0.5f, 2.6f), Quaternion.identity);

        isInitialized = true;
      }

      if (nextSend < Time.time) {
        nextSend = Time.time + SEND_RATE;
        logger.Log("LOG (server): Sending world update");
        udpServer.SendMessage(localObjectStore.Serialize());
      }
    }

    void DoTriggerPressed(object sender, ControllerInteractionEventArgs e) {
      localObjectStore.Instantiate(PrefabId.SPHERE, controller.transform.position, controller.transform.rotation);
    }
  }
}