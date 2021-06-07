using UnityEngine;

public static class ColorUtils
{
  
  private static Color RedColor = new Color(0.94f,0.28f,0.44f);
  private static Color BlueColor = new Color(0.07f,0.54f,0.7f);
  private static Color YellowColor = new Color(1f,0.82f,0.4f);
  private static Color GreenColor = new Color(0.02f,0.84f,0.63f);

  public static Color GetPlayerColor(DPlayer.ID playerID)
  {
    switch (playerID)
    {
      case DPlayer.ID.Red:
        return RedColor;
      case DPlayer.ID.Blue:
        return BlueColor;
      case DPlayer.ID.Yellow:
        return YellowColor;
      case DPlayer.ID.Green:
        return GreenColor;
      default:
        return Color.white;
    }
  }
  
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