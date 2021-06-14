using System;
using UnityEngine;

public class Observable<T>
{
  public Action<T> OnChange = v => {};

  private T _current;
  public T Current {
    get => _current;
    set
    {
      _current = value;
      OnChange(value);
    }
  }

  public Observable(T initial)
  {
    _current = initial;
  }

  public Observable<U> Map<U>(Func<T, U> map)
  {
    Observable<U> mapped = new Observable<U>(map(_current));
    mapped.Current = map(_current);
    OnChange += value => mapped.Current = map(value);

    return mapped;
  }
}
