﻿using UnityEngine;
using NetworkingFBS;
using Assets.Scripts.Buffering;
using System.Collections.Generic;
using System.Linq;
using FlatBuffers;

namespace Assets.Scripts {
  public class SecondaryTransition {
    public NetworkedObject obj;
    public StateTransition<FlatNetworkedObject> transition;

    public SecondaryTransition(NetworkedObject o, StateTransition<FlatNetworkedObject> t) {
      obj = o;
      transition = t;
    }
  }

  public class SecondaryManager : MonoBehaviour {
    static ILogger logger = Debug.logger;

    public LocalObjectStore localObjectStore;

    UDPClient udpClient;
    StateBuffer<FlatWorldState> stateBuffer;
    StateTransition<FlatWorldState> worldTransition;
    List<SecondaryTransition> secondaryTransitions;

    float UPDATE_RATE = 0.033F;
    float nextUpdateTime = 0.0F;

    void Start() {
      udpClient = new UDPClient();
      udpClient.Connect();

      stateBuffer = new StateBuffer<FlatWorldState>();
      worldTransition = null;
      secondaryTransitions = new List<SecondaryTransition>();
    }

    void Update() {
      byte[] bytes = udpClient.Read();
      if (bytes != null) {
        FlatWorldState receivedWorldState = Serializer.BytesToFlatWorldState(bytes);
        for (var i = 0; i < receivedWorldState.PrimariesLength; i++) {
          var receivedPrimary = receivedWorldState.GetPrimaries(i);
          var secondary = localObjectStore.GetSecondary(receivedPrimary.Guid);
          if (secondary == null) {
            // Secondary copy does not exist yet. Create one and register it.
            localObjectStore.Instantiate(
              (PrefabId)receivedPrimary.PrefabId,
              Serializer.DeserializeVector3(receivedPrimary.Position),
              Serializer.DeserializeQuaternion(receivedPrimary.Rotation),
              receivedPrimary.Guid
            );
          } else {
            // Add state update to secondary object's StateBuffer.
            secondary.buffer.Enqueue(receivedPrimary);
          }
        }
      }

      // When a lerping time is up, we get the newest positions to lerp towards.
      if (nextUpdateTime < Time.time) {
        secondaryTransitions = localObjectStore
            .GetSecondaries()
            .Where(sec => sec.isInitialized)
            .Select(sec => new SecondaryTransition(sec, sec.buffer.Dequeue()))
            .Where(trans => trans.transition != null).ToList();
      }

      var dirtyObjects = new List<NetworkedObject>();
      float primaryTime = 0;
      foreach (var transition in secondaryTransitions) {
        // Send a timestamp that is relative to the primary's clock.
        primaryTime = transition.transition.last.Timestamp + (nextUpdateTime - Time.time);
        if (transition.obj.HasMoved()) {
          // The object has been moved by the player, so free it from the lerp loop for this cycle.
          secondaryTransitions.Remove(transition);
          dirtyObjects.Add(transition.obj);
        } else {
          // Interpolate object between updates from the primary.
          transition.obj.Lerp(transition.transition, (nextUpdateTime - Time.time) / UPDATE_RATE);
        }
      }

      // Update the primary owner about every moved object.
      if (dirtyObjects.Count > 0) {
        logger.Log(dirtyObjects.Count + " secondary objects were dirty");
        var builder = new FlatBufferBuilder(1024);
        var worldState = localObjectStore.SerializeSecondaries(builder, dirtyObjects, primaryTime);
        udpClient.SendMessage(Serializer.FlatWorldStateToBytes(builder, worldState));
      }
    }
  }
}
