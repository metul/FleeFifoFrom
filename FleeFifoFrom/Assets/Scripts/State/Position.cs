using System;

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

    if (!IsValid)
    {
      throw new System.Exception($"Invalid Position: {Row}, {Col}");
    }
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

  // TODO: also perhaps we need a CanJumpTo() for kids?
  // TODO: also perhaps NeighboringPositions()?

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
}
