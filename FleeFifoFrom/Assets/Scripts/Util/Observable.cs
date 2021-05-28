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

  public Observable<U> Map<U>(Func<T, U> map)
  {
    Observable<U> mapped = new Observable<U>(map(Current));
    OnChange += value => mapped.Current = map(value);

    return mapped;
  }
}
