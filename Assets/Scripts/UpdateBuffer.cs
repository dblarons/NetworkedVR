using Lidgren.Network;

namespace Assets.Scripts {
  public class UpdateLerp<T> where T : class {
    public T last { get; set; }
    public T next { get; set; }

    public UpdateLerp(T last, T next) {
      this.last = last == null ? next : last;
      this.next = next;
    }
  }

  public class UpdateBuffer<T> where T : class {
    readonly int CAPACITY = 3;

    NetQueue<T> queue;
    T lastUpdate;

    public UpdateBuffer() {
      queue = new NetQueue<T>(CAPACITY);
    }

    /**
     * Given a new item, add it to the queue if the count is under capacity. 
     * If count is at capacity, dequeue and throw away the oldest item first before
     * queuing the new item.
     */
    public void Enqueue(T item) {
      if (queue.Count == CAPACITY) {
        T throwaway;
        queue.TryDequeue(out throwaway);
      }

      queue.Enqueue(item);
    }

    /**
     * First, dequeue an item from the queue. If no item can be dequeued, we cannot
     * update the world state, so return null. If an item can be dequeued, we return
     * the updated state along with a reference to the last update we performed. We
     * then update the last item reference to the newest item we dequeued.
     */
    public UpdateLerp<T> Dequeue() {
      T nextUpdate = null;
      queue.TryDequeue(out nextUpdate);

      UpdateLerp<T> updateLerp = nextUpdate == null
          ? null
          : new UpdateLerp<T>(lastUpdate, nextUpdate);

      // Keep track of the last update.
      lastUpdate = nextUpdate;
      return updateLerp;
    }
  }
}
