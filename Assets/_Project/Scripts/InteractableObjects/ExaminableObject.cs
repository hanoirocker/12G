namespace TwelveG.InteractableObjects
{
  using TwelveG.Localization;
  using TwelveG.PlayerController;
  using TwelveG.UIController;
  using UnityEngine;
  using UnityEngine.EventSystems;

  public class ExaminableObject : MonoBehaviour, IDragHandler
  {
    [Header("References")]
    [SerializeField] private AudioClip inspectionClip;

    [Header("Examination Texts SO's")]
    [SerializeField] private ExaminationTextSO examinationTextSO;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onPlayerControls;
    [SerializeField] private GameEventSO onExaminationCanvasShowText;
    [SerializeField] private GameEventSO onExaminationCanvasControls;

    [Header("Settings")]
    [SerializeField, Range(0.1f, 0.5f)]private float rotationSpeed = 0.5f;

    private Vector3 initialMousePosition;
    private Vector3 initialRotation;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!canvasIsShowing)
      {
        initialMousePosition = Input.mousePosition;
        initialRotation = transform.eulerAngles;
      }
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (!canvasIsShowing)
      {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 mouseDelta = currentMousePosition - initialMousePosition;

        // Rotación más suave y controlada
        Vector3 newRotation = new Vector3(
            initialRotation.x - mouseDelta.y * rotationSpeed,
            initialRotation.y + mouseDelta.x * rotationSpeed,
            initialRotation.z
        );

        transform.eulerAngles = newRotation;
      }
    }
  }
}