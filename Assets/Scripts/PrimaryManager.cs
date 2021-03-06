﻿using FlatBuffers;
using NetworkingFBS;
using UnityEngine;

namespace Assets.Scripts {
  public class PrimaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public LocalObjectStore localObjectStore;

    UDPServer udpServer;

    float SEND_RATE = 0.033F;
    float nextSend = 0.0F;
    bool isInitialized = false;

    // MEASUREMENTS
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    public float m_refreshTime = 10.0f;

    void Start() {
      udpServer = new UDPServer();
    }

    void Update() {
      if (!isInitialized) {
        localObjectStore.Instantiate(PrefabId.CUBE, new Vector3(-0.1f, 0.5f, 2.6f), Quaternion.identity);
        localObjectStore.Instantiate(PrefabId.SPHERE, new Vector3(0.1f, 0.5f, 2.6f), Quaternion.identity);

        isInitialized = true;
      }

      byte[] bytes = udpServer.Read();
      if (bytes != null) {
        FlatWorldState receivedWorldState = Serializer.BytesToFlatWorldState(bytes);
        for (int i = 0; i < receivedWorldState.SecondariesLength; i++) {
          logger.Log("Primary received and set " + receivedWorldState.SecondariesLength + " dirty objects from secondary");
          // When a secondary update is received, immediately set the primary to the extrapolated
          // secondary position.
          var receivedSecondary = receivedWorldState.GetSecondaries(i);
          var primary = localObjectStore.GetPrimary(receivedSecondary.Guid);
          logger.Log("Extrapolated time was: " + (Time.time - receivedSecondary.Timestamp));
          primary.Extrapolate(receivedSecondary, Time.time - receivedSecondary.Timestamp);
        }
      }

      if (nextSend < Time.time) {
        nextSend = Time.time + SEND_RATE;

        // TODO(dblarons): Dynamically allocate this size by asking the localObjectStore for it.
        var builder = new FlatBufferBuilder(1024);
        var worldState = localObjectStore.SerializePrimaries(builder, Time.time);
        udpServer.SendMessage(Serializer.FlatWorldStateToBytes(builder, worldState));
      }

      // MEASUREMENTS
      if( m_timeCounter < m_refreshTime ) {
        m_timeCounter += Time.deltaTime;
        m_frameCounter++;
      } else {
        m_lastFramerate = (float)m_frameCounter/m_timeCounter;
        m_frameCounter = 0;
        m_timeCounter = 0.0f;
        logger.Log("[MEASURE] Frame rate: " + m_lastFramerate);
      }
    }

    void OnApplicationQuit() {
      logger.Log("Application quit: cleaning up resources");
      udpServer.Shutdown();
    }
  }
}
