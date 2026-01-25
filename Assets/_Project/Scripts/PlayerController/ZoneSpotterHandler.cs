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
    public bool hasBeenSpotted = false;

    [Header("References")]
    [Space]
    [SerializeField] private GameEventSO eventToTrigger;

    private void OnEnable()
    {
      hasBeenSpotted = false;
      canBeSpotted = true;
    }

    public void SpotOnObject(bool playerIsZooming)
    {
      if (!needsToBeZoomed || (needsToBeZoomed && playerIsZooming))
      {
        // No desactivamos el objeto porque puede ser necesario que el jugador lo siga viendo 
        // en pantalla aunque ya lo haya spotteado (ejemplo: Enemigo)
        // Eventualmente para colliders se desactiva desde el listener del evento
        if (disableAfterSpotted) canBeSpotted = false;

        Debug.Log("Zone spotted!");
        hasBeenSpotted = true;
        eventToTrigger?.Raise(this, null);
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