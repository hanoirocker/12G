namespace TwelveG.AudioController
{
  using UnityEngine;

  public class AcousticZone : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private Transform audioZoneTransform;

    [Header("Settings")]
    public bool simulateOpenZone = false;


    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        // Debug.Log($"Entered zone {gameObject.name}");
        AudioManager.Instance.AZHandler.EnteredAcousticZone(audioZoneTransform);

        // Desactivar corte de agudos en canal "Ambient"
        if (simulateOpenZone)
        {
          AudioManager.Instance.EnableLowPassOnAmbientChannel(false);
        }
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        // Debug.Log($"Exited zone {gameObject.name}");
        AudioManager.Instance.AZHandler.ExitedAcousticZone(audioZoneTransform);

        // Reactivar corte de agudos en canal "Ambient"
        if (simulateOpenZone)
        {
          AudioManager.Instance.EnableLowPassOnAmbientChannel(true);
        }
      }
    }
  }
}