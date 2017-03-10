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

    public void Enqueue(T item) {
      if (queue.Count == CAPACITY) {
        T throwaway;
        queue.TryDequeue(out throwaway);
      }

      queue.Enqueue(item);
    }

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
