using UnityEngine;

namespace TwelveG.PlayerController
{
  public class ZoneSpotterHandler : MonoBehaviour, ISpot
  {
    [Header("Settings")]
    [Space]
    public bool canBeSpotted = true;
    [Space(5)]
    public bool disableAfterSpotted = false;
    public bool needsToBeZoomed = true;

    [Header("References")]
    [Space]
    [SerializeField] private GameEventSO eventToTrigger;

    public void SpotOnObject(bool playerIsZooming)
    {
      if (!needsToBeZoomed || (needsToBeZoomed && playerIsZooming))
      {
        Debug.Log("Zone spotted!");

        if (disableAfterSpotted) canBeSpotted = false;

        eventToTrigger.Raise(this, null);
      }
    }

    public bool CanBeSpotted()
    {
      return canBeSpotted;
    }

    public void IsAbleToBeSpotted(bool isAble)
    {
      canBeSpotted = isAble;
    }
  }
}