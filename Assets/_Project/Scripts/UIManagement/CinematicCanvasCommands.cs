namespace TwelveG.UIManagement
{
  public abstract class CinematicCanvasCommandsBase { }

  public class ShowCinematicBars : CinematicCanvasCommandsBase
  {
    public bool Show;

    public ShowCinematicBars(bool show) => Show = show;
  }
}