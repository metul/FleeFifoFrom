using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class DPosition
{
  public readonly ushort Row;

  public readonly ushort Col;

  public bool IsValid
  {
    get
    {
      return Row > 0 && Col > 0 && Row <= Rules.ROWS && Col <= Row;
    }
  }

  public bool IsFinal
  {
    get => Row == 1 && Col == 1;
  }

  public DPosition(ushort row, ushort col)
  {
    Row = row;
    Col = col;
  }

  public bool CanMoveTo(DPosition other)
  {
    return (
      IsValid &&
      other.IsValid &&
      other.Row == Row - 1 &&
      (other.Col == Col - 1 || other.Col == Col)
    );
  }

  public bool Neighbors(DPosition other)
  {
    return (
      IsValid &&
      other.IsValid &&
      (
        (Row == other.Row && Math.Abs(Col - other.Col) <= 1) ||
        CanMoveTo(other) || other.CanMoveTo(this)
      ) &&
      !Equals(other)
    );
  }

  public DPosition[] Predecessors()
  {
    return (new DPosition[]{
      new DPosition((ushort) (Row + 1), (ushort) (Col + 1)),
      new DPosition((ushort) (Row + 1), Col),
    }).Where(p => p.IsValid).ToArray();
  }

  public DPosition[] Successors()
  {
    return (new DPosition[]{
      new DPosition((ushort) (Row - 1), (ushort) (Col - 1)),
      new DPosition((ushort) (Row - 1), Col),
    }).Where(p => p.IsValid).ToArray();
  }

  public bool Equals(DPosition other)
  {
    return (
      IsValid && other.IsValid && other.Row == Row && other.Col == Col
    );
  }

  public override string ToString()
  {
    return $"[{Row}, {Col}]";
  }

  public static DPosition[] GetRow(ushort row)
  {
    List<DPosition> res = new List<DPosition>();
    for (ushort col = 1; col <= row; col++)
    {
      res.Add(new DPosition(row, col));
    }

    return res.ToArray();
  }

  public static DPosition[] LastRow()
  {
    return GetRow(Rules.ROWS);
  }
}
