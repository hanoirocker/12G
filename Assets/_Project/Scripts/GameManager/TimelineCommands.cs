namespace TwelveG.GameController
{
  public abstract class TimelineCommandsBase { }

  public class ToggleTimelineDirector
  {
    public int Index;
    public bool Enable;

    public ToggleTimelineDirector(int index, bool enable)
      => (Index, Enable) = (index, enable);
  }
}