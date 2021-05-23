using System;

public class Observable<T>
{
  public Action<T> OnChange = v => {};

  private T _current;
  public T Current {
    get => _current;
    set
    {
      _current = value;
      OnChange(_current);
    }
  }

  public Observable(T initial)
  {
    _current = initial;
  }
}
