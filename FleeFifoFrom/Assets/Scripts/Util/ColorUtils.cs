using UnityEngine;

public static class ColorUtils
{
  public static Color TextColor(this Color background)
  {
    if (background.IsLight())
        return Color.black;
    else
        return Color.white;
  }

  public static float Luminance(this Color color)
  {
    return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
  }

  public static bool IsLight(this Color color)
  {
    return color.Luminance() > 0.5f;
  }

  public static bool IsDark(this Color color)
  {
    return !color.IsLight();
  }
}