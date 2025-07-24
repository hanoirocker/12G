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
        EnvironmentAudioController.Instance.EnteredAcousticZone(audioZoneTransform);
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("Player"))
      {
        Debug.Log($"Exited zone {gameObject.name}");
        EnvironmentAudioController.Instance.ExitedAcousticZone(audioZoneTransform);
      }
    }
  }
}