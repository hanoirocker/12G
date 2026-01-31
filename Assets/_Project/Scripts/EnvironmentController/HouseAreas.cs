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
        // Escuchan PlayerHandler y EnvironmentAudioHandler.
        GameEvents.Common.onPlayerEnteredHouseArea.Raise(this, houseArea);
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        GameEvents.Common.onPlayerExitedHouseArea.Raise(this, houseArea);
      }
    }
  }
}