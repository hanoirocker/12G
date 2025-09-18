namespace TwelveG.AudioController
{
  using UnityEngine;

  public class AcousticZone : MonoBehaviour
  {
    [SerializeField] private Transform audioZoneTransform;

    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("Player"))
      {
        Debug.Log($"Entered zone {gameObject.name}");
        AudioManager.Instance.RainHandler.EnteredAcousticZone(audioZoneTransform);
      }
    }
 
    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("Player"))
      {
        Debug.Log($"Exited zone {gameObject.name}");
        AudioManager.Instance.RainHandler.EnteredAcousticZone(audioZoneTransform);
      }
    }
  }
}