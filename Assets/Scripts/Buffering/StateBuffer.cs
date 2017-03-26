using Lidgren.Network;

namespace Assets.Scripts.Buffering {
  public class StateBuffer<T> where T : class {
    readonly int CAPACITY = 3;

    NetQueue<T> queue;
    T lastUpdate;

    public StateBuffer() {
      queue = new NetQueue<T>(CAPACITY);
    }

    /// <summary>
    /// Given a new item, add it to the queue if the count is under capacity. 
    /// If count is at capacity, dequeue and throw away the oldest item first before
    /// queuing the new item.
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(T item) {
      if (queue.Count == CAPACITY) {
        T throwaway;
        queue.TryDequeue(out throwaway);
      }

      queue.Enqueue(item);
    }

    /// <summary>
    /// First, dequeue an item from the queue. If no item can be dequeued, we cannot
    /// update the world state, so return null. If an item can be dequeued, we return
    /// the updated state along with a reference to the last update we performed. We
    /// then update the last item reference to the newest item we dequeued.
    /// </summary>
    /// <returns></returns>
    public StateTransition<T> Dequeue() {
      T nextUpdate = null;
      queue.TryDequeue(out nextUpdate);

      StateTransition<T> updateLerp = nextUpdate == null
          ? null
          : new StateTransition<T>(lastUpdate, nextUpdate);

      // Keep track of the last update.
      lastUpdate = nextUpdate;
      return updateLerp;
    }
  }
}
