namespace TwelveG.PlayerController
{
  interface ISpot
  {
    public void SpotOnObject();

    public bool CanBeSpotted();

    public void IsAbleToBeSpotted(bool isAble);
  }
}