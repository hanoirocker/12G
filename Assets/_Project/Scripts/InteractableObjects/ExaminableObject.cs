namespace TwelveG.InteractableObjects
{
  using TwelveG.Localization;
  using TwelveG.PlayerController;
  using UnityEngine;

  public class ExaminableObject : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private AudioClip inspectionClip;

    [Header("Observation Texts SO's")]
    // [SerializeField] private ObservationTextSO observationTextSO;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onPlayerControls;

    private bool canBeExamined = true;

    void Start()
    {
      onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
      onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));
      onPlayerControls.Raise(this, new EnablePauseMenuCanvasAccess(false));

      PlayExaminationSound();
    }

    void OnDestroy()
    {
      PlayExaminationSound();

      onPlayerControls.Raise(this, new EnablePlayerControllers(true));
      onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
      onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
      onPlayerControls.Raise(this, new EnablePauseMenuCanvasAccess(true));
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        Destroy(gameObject);
      }
      if (Input.GetKeyDown(KeyCode.E))
      {
        Debug.Log("Abriendo Canvas");
      }
    }

    private void PlayExaminationSound()
    {
      AudioSource _audioSource = GetComponent<AudioSource>();
      if (inspectionClip != null)
      {
        if (_audioSource.isPlaying) { _audioSource.Stop(); }
        _audioSource.PlayOneShot(inspectionClip);
      }
    }
  }
}