namespace Assets.Scripts.Buffering {
  public class StateTransition<T> where T : class {
    public T last { get; set; }
    public T next { get; set; }

    public StateTransition(T last, T next) {
      this.last = last == null ? next : last;
      this.next = next;
    }
  }
}
