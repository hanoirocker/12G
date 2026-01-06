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
        Debug.Log("Zone spotted!");
        if (disableAfterSpotted) canBeChecked = false;

        eventToTrigger.Raise(this, null);

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