public class DPosition {
  public readonly ushort Row;
  public readonly ushort Col;

  public bool IsValid {
    get {
      return Row > 0 && Col > 0 && Row <= Rules.ROWS && Col <= Row;
    }
  }

  public DPosition(ushort row, ushort col) {
    Row = row;
    Col = col;

    if (!IsValid) {
      throw new System.Exception($"Invalid Position: {Row}, {Col}");
    }
  }

  public bool CanMoveTo(DPosition other) {
    return (
      IsValid &&
      other.IsValid &&
      other.Row == Row - 1 &&
      (other.Col == Col + 1 || other.Col == Col - 1)
    );
  }

  public bool Equals(DPosition other) {
    return (
      IsValid && other.IsValid && other.Row == Row && other.Col == Col
    );
  }

  // TODO: also perhaps we need a CanJumpTo() for kids?
}
