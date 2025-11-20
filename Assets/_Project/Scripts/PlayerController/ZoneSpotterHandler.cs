using UnityEngine;

namespace TwelveG.PlayerController
{
  public class ZoneSpotterHandler : MonoBehaviour, ISpot
  {
    private bool canBeChecked = true;

    public void SpotOnObject()
    {
      throw new System.NotImplementedException();
    }

    public bool CanBeSpotted()
    {
      return canBeChecked;
    }

    public void IsAbleToBeSpotted(bool isAble)
    {
      canBeChecked = isAble;
    }
  }
}