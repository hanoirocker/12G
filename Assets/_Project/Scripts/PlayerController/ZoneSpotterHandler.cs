using UnityEngine;

namespace TwelveG.PlayerController
{
  public class ZoneSpotterHandler : MonoBehaviour, ISpot
  {
    [Header("Settings")]
    [Space]
    public bool disableAfterSpotted = false;
    public bool destroyAfterSpotted = false;

    [Header("References")]
    [Space]
    [SerializeField] private GameEventSO eventToTrigger;

    private bool canBeChecked = true;

    public void SpotOnObject()
    {
      if (canBeChecked)
      {
        eventToTrigger.Raise(this, null);
        if (disableAfterSpotted) canBeChecked = false;
        if (destroyAfterSpotted) Destroy(this.gameObject);
      }
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