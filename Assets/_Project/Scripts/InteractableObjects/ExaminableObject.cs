namespace TwelveG.InteractableObjects
{
  using System.Collections;
  using TwelveG.AudioController;
  using TwelveG.Localization;
  using TwelveG.PlayerController;
  using TwelveG.UIController;
  using TwelveG.Utils;
  using UnityEngine;
  using UnityEngine.EventSystems;

  public class ExaminableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    [Header("References")]
    [SerializeField] private AudioClip examineInClip;
    [SerializeField] private AudioClip examineOutClip;

    [Header("Examination Texts SO's")]
    [SerializeField] private ExaminationTextSO examinationTextSO;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onObjectExamined;
    [SerializeField] private GameEventSO onPlayerControls;
    [SerializeField] private GameEventSO onExaminationCanvasShowText;
    [SerializeField] private GameEventSO onExaminationCanvasControls;

    [Header("Rotation Settings")]
    [SerializeField, Range(0.1f, 5f)] private float rotationSpeed = 1f;
    [SerializeField] private bool invertX = true;
    [SerializeField] private bool invertY = true;

    public bool canBeExamined = false;

    private AudioSource interactionSource;
    private Vector2 lastMousePosition;
    private bool isDragging = false;
    private bool canvasIsShowing = false;

    private void Awake()
    {
      interactionSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);
    }

    void Start()
    {
      PlayExaminationSoundCoroutine(examineInClip);
      onPlayerControls.Raise(this, new ToggleToObjectExamination(true));
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        PlayExaminationSoundCoroutine(examineOutClip);
        StartCoroutine(DestroyAfterSound());
      }
      if (Input.GetKeyDown(KeyCode.E))
      {
        ToggleCanvas();
      }
    }

    private void ToggleCanvas()
    {
      if (!canvasIsShowing)
      {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        onExaminationCanvasShowText.Raise(this, examinationTextSO);
      }
      else
      {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        onExaminationCanvasControls.Raise(this, new EnableCanvas(false));
      }
      canvasIsShowing = !canvasIsShowing;
    }

    private IEnumerator DestroyAfterSound()
    {
      yield return null;

      var mainCameraHandler = GetComponentInParent<MainCameraHandler>();
      if (mainCameraHandler != null && mainCameraHandler.lastEventSender != null)
      {
        var examinationHandler = mainCameraHandler.lastEventSender.GetComponent<ObjectExaminationHandler>();
        if (examinationHandler != null)
        {
          examinationHandler.ShowObjectInScene(true);
        }
        mainCameraHandler.lastEventSender = null;
      }

      // Si se asignó un event SO a disparar luego de dejar de inspeccionar, dispararlo
      if (onObjectExamined != null)
      {
        onObjectExamined.Raise(this, null);
      }

      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      if (canvasIsShowing) { onExaminationCanvasControls.Raise(this, new EnableCanvas(false)); }
      onPlayerControls.Raise(this, new ToggleToObjectExamination(false));
      Destroy(gameObject);
    }

    private void PlayExaminationSoundCoroutine(AudioClip inspectionClip)
    {
      if (interactionSource == null)
      {
        Debug.LogWarning("[ExaminableObject]: No audio source available for playing the inspectionClip.");
        return;
      }

      if (inspectionClip == null)
      {
        Debug.LogWarning("[ExaminableObject]: inspectionClip not assigned!");
        return;
      }

      if (interactionSource.isPlaying)
      {
        interactionSource.Stop();
      }
      interactionSource.PlayOneShot(inspectionClip);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!canvasIsShowing)
      {
        isDragging = true;
        lastMousePosition = eventData.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
      }
    }

    public void OnDrag(PointerEventData eventData)
    {
      DirectRotation(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      isDragging = false;
      Cursor.visible = true;
    }

    private void DirectRotation(PointerEventData eventData)
    {
      if (!canvasIsShowing && isDragging)
      {
        Vector2 currentMousePosition = eventData.position;
        Vector2 mouseDelta = currentMousePosition - lastMousePosition;

        // Aplicar inversión según configuración
        float xRotation = mouseDelta.y * rotationSpeed * (invertY ? 1 : -1);
        float yRotation = mouseDelta.x * rotationSpeed * (invertX ? -1 : 1);

        // Rotación directa (más responsive)
        transform.Rotate(xRotation, yRotation, 0, Space.World);

        lastMousePosition = currentMousePosition;
      }
    }
  }
}