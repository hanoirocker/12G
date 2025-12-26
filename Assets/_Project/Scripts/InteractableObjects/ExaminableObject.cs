using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwelveG.InteractableObjects
{
  // [RequireComponent] asegura que el objeto tenga colisionador para los eventos de Drag
  [RequireComponent(typeof(Collider))]
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

    [Header("Rotation Settings")]
    [SerializeField, Range(0.1f, 5f)] private float rotationSpeed = 1f;
    [SerializeField] private bool invertX = true;
    [SerializeField] private bool invertY = true;
    [SerializeField, Tooltip("Máxima velocidad de rotación en grados por segundo (se aplica como clamp por frame).")]
    private float maxDegreesPerSecond = 60f;

    public bool canBeExamined = false;

    private AudioSource interactionSource;
    private AudioSourceState audioSourceState;
    private Vector2 lastMousePosition;

    private bool isDragging = false;
    private bool canvasIsShowing = false;

    // Caché para evitar Allocations en tiempo de ejecución
    private Renderer[] cachedRenderers;

    private void Awake()
    {
      // Cacheamos los renderers una sola vez al inicio
      cachedRenderers = GetComponentsInChildren<Renderer>();

      // Obtenemos el AudioSource del pool
      if (AudioManager.Instance != null)
      {
        (interactionSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
      }
    }

    void Start()
    {
      if (examineInClip != null)
      {
        StartCoroutine(PlayExaminationSoundCoroutine(examineInClip));
      }

      // Disparamos eventos de configuración inicial
      GameEvents.Common.onExaminationCanvasControls.Raise(this, examinationTextSO);
      GameEvents.Common.onExaminationCanvasControls.Raise(this, new EnableCanvas(true));
      GameEvents.Common.onPlayerControls.Raise(this, new ToggleToObjectExamination(true));

      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        StartCoroutine(StopInspectingRoutine());
      }
      else if (Input.GetKeyDown(KeyCode.E) && examinationTextSO != null)
      {
        ToggleCanvas();
      }
    }

    private void ToggleCanvas()
    {
      canvasIsShowing = !canvasIsShowing;

      if (canvasIsShowing) // Mostrando texto
      {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameEvents.Common.onExaminationCanvasShowText.Raise(this, true);
      }
      else // Ocultando texto (volviendo a examinar)
      {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameEvents.Common.onExaminationCanvasShowText.Raise(this, false);
      }
    }

    private IEnumerator StopInspectingRoutine(bool byForce = false)
    {
      // Reproducir sonido de salida
      if (examineOutClip != null && !byForce)
      {
        StartCoroutine(PlayExaminationSoundCoroutine(examineOutClip));
      }

      // Restaurar objeto original en la escena
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

      if (onObjectExamined != null)
      {
        onObjectExamined.Raise(this, null);
      }

      // Restaurar cursor y controles
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      GameEvents.Common.onExaminationCanvasControls.Raise(this, new EnableCanvas(false));
      GameEvents.Common.onPlayerControls.Raise(this, new ToggleToObjectExamination(false));

      if (cachedRenderers != null)
      {
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
          if (cachedRenderers[i] != null) cachedRenderers[i].enabled = false;
        }
      }

      if (interactionSource != null && interactionSource.isPlaying)
      {
        float remainingTime = interactionSource.clip.length - interactionSource.time;
        if (remainingTime > 0) yield return new WaitForSeconds(remainingTime);
      }

      if (!byForce)
      {
        AudioUtils.StopAndRestoreAudioSource(interactionSource, audioSourceState);
        interactionSource = null;
      }

      Destroy(gameObject);
    }

    private IEnumerator PlayExaminationSoundCoroutine(AudioClip inspectionClip)
    {
      if (interactionSource == null) yield break;

      interactionSource.Stop();
      interactionSource.clip = inspectionClip;
      interactionSource.pitch = Random.Range(0.8f, 1.2f);
      interactionSource.Play();
      yield return null;
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
      if (!canvasIsShowing)
      {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
      }
    }

    public void CancelExaminationMode()
    {
      StartCoroutine(StopInspectingRoutine(true));
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

        // Limitar la velocidad de rotación por frame para evitar giros bruscos
        float maxPerFrame = maxDegreesPerSecond * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -maxPerFrame, maxPerFrame);
        yRotation = Mathf.Clamp(yRotation, -maxPerFrame, maxPerFrame);

        // Rotación directa (más responsive)
        transform.Rotate(xRotation, yRotation, 0, Space.World);

        lastMousePosition = currentMousePosition;
      }
    }
  }
}