namespace TwelveG.GameController
{
  public interface ICheckpointListener
  {
    void OnCheckpointReached(string state);
  }
}