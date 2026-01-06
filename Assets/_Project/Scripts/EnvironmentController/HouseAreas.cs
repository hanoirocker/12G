namespace TwelveG.EnvironmentController
{
  using TwelveG.GameController;
  using UnityEngine;

  public class HouseAreas : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private HouseArea houseArea;

    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        // Escucha el Player Handler para que cualquier script pueda obtener 
        // la posición del jugador en la casa.
        // También posiblemente a futuroel Audio Controller para cambiar audios de ambiente.
        GameEvents.Common.onPlayerEnteredHouseArea.Raise(this, houseArea);
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        // Debug.Log($"Exited zone {houseArea}");
      }
    }
  }
}