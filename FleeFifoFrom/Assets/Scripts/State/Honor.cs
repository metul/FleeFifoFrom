using System;
using UnityEngine;

public class DHonor
{
  public Observable<ushort> Index { get; protected set; }
  public Observable<short> Score { get; protected set; }

  public DHonor()
  {
    Index = new Observable<ushort>((ushort) (Rules.HONOR_VALUES.Length / 2));
    Score = Index.Map(index => Rules.HONOR_VALUES[index]);
  }

  public void Earn(ushort points = 1)
  {
    Index.Current = (ushort) Math.Min(Index.Current + points, Rules.HONOR_VALUES.Length - 1);
  }

  public void Lose(ushort points = 1)
  {
    Index.Current = (ushort) Math.Max(Index.Current - points, 0);
  }
}