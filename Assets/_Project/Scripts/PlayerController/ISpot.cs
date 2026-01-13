namespace TwelveG.PlayerController
{
  interface ISpot
  {
    public void SpotOnObject(bool playerIsZooming);

    public bool CanBeSpotted();

    public void IsAbleToBeSpotted(bool isAble);
  }
}