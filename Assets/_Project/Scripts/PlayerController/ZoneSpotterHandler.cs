using UnityEngine;

namespace TwelveG.PlayerController
{
  public class ZoneSpotterHandler : MonoBehaviour, ISpot
  {
    [Header("Settings")]
    [Space]
    public bool disableAfterSpotted = false;
    public bool triggerEventAfterSpotted = false;

    [Header("References")]
    [Space]
    [SerializeField] private GameEventSO eventToTrigger;

    private bool canBeChecked = true;

    public void SpotOnObject()
    {
      if(triggerEventAfterSpotted) eventToTrigger.Raise(this, null);
      if(disableAfterSpotted) canBeChecked = false;
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