namespace TwelveG.UIController
{
  public enum FadeType
  {
    FadeIn,
    FadeOut
  }

  public abstract class ImageCanvasCommandsBase { }

  public class WakeUpBlinking : ImageCanvasCommandsBase { }

  public class FadeImage : ImageCanvasCommandsBase
  {
    public FadeType FadeType;
    public float Duration;

    public FadeImage(FadeType fadeType, float duration)
    {
      FadeType = fadeType;
      Duration = duration;
    }
  }
}