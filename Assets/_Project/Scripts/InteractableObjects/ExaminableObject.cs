using System.Collections;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwelveG.InteractableObjects
{
  public class ExaminableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    [Header("Audio Settings")]
    [SerializeField] private AudioClip examineInClip;
    [SerializeField] private AudioClip examineOutClip;
    [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.7f;

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
    private AudioSourceState audioSourceState;
    private Vector2 lastMousePosition;
    private bool isDragging = false;
    private bool canvasIsShowing = false;

    private void Awake()
    {
      (interactionSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
    }

    void Start()
    {
      StartCoroutine(PlayExaminationSoundCoroutine(examineInClip));
      onPlayerControls.Raise(this, new ToggleToObjectExamination(true));
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        StartCoroutine(StopInspectingRoutine());
      }
      if (Input.GetKeyDown(KeyCode.E) && examinationTextSO != null)
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

    private IEnumerator StopInspectingRoutine()
    {
      StartCoroutine(PlayExaminationSoundCoroutine(examineOutClip));

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
      gameObject.GetComponent<Renderer>().enabled = false;

      yield return new WaitUntil(() => !interactionSource.isPlaying);
      Destroy(gameObject);
    }

    private IEnumerator PlayExaminationSoundCoroutine(AudioClip inspectionClip)
    {
      interactionSource.clip = inspectionClip;
      interactionSource.pitch = Random.Range(0.8f, 1.2f);
      interactionSource.Play();
      yield return new WaitUntil(() => !interactionSource.isPlaying);
      AudioUtils.StopAndRestoreAudioSource(interactionSource, audioSourceState);
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