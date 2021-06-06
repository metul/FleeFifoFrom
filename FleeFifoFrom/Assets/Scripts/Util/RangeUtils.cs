public static class RangeUtils
{
  public static ushort Normalize(int index, int length)
  {
    while (index < 0)
    {
      index += length;
    }

    while (index >= length)
    {
      index -= length;
    }

    return (ushort) index;
  }
}