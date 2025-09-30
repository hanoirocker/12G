namespace TwelveG.InteractableObjects
{
  using TwelveG.Localization;
  using TwelveG.PlayerController;
  using TwelveG.UIController;
  using UnityEngine;

  public class ExaminableObject : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private AudioClip inspectionClip;

    [Header("Examination Texts SO's")]
    [SerializeField] private ExaminationTextSO examinationTextSO;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onPlayerControls;
    [SerializeField] private GameEventSO onExaminationCanvasShowText;
    [SerializeField] private GameEventSO onExaminationCanvasControls;

    private bool canBeExamined = true;
    private bool canvasIsShowing = false;

    void Start()
    {
      onPlayerControls.Raise(this, new ToggleToObjectExamination(true));
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
      PlayExaminationSound();
    }

    void OnDestroy()
    {
      PlayExaminationSound();
      onPlayerControls.Raise(this, new ToggleToObjectExamination(false));
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (canvasIsShowing) { onExaminationCanvasControls.Raise(this, new EnableCanvas(false)); }
        Destroy(gameObject);
      }
      if (Input.GetKeyDown(KeyCode.E))
      {
        if (!canvasIsShowing)
        {
          Cursor.visible = false;
          Cursor.lockState = CursorLockMode.Locked;
          onExaminationCanvasShowText.Raise(this, examinationTextSO);
        }
        else
        {
          Debug.Log("Ocultando canvas para rotar objeto nuevamente!");
          Cursor.visible = true;
          Cursor.lockState = CursorLockMode.None;
          onExaminationCanvasControls.Raise(this, new EnableCanvas(false));
        }
        canvasIsShowing = !canvasIsShowing;
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