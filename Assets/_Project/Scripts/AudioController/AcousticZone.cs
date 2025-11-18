namespace TwelveG.AudioController
{
  using UnityEngine;

  public class AcousticZone : MonoBehaviour
  {
    [SerializeField] private Transform audioZoneTransform;

    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        // Debug.Log($"Entered zone {gameObject.name}");
        AudioManager.Instance.AZHandler.EnteredAcousticZone(audioZoneTransform);
      }
    }
 
    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("PlayerCapsule"))
      {
        // Debug.Log($"Exited zone {gameObject.name}");
        AudioManager.Instance.AZHandler.ExitedAcousticZone(audioZoneTransform);
      }
    }
  }
}