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

  public class ExaminableObject : MonoBehaviour, IDragHandler
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

    [Header("Settings")]
    [SerializeField, Range(0.1f, 0.5f)] private float rotationSpeed = 0.5f;
    public bool canBeExamined = false;

    private AudioSource interactionSource;
    private Vector3 initialMousePosition;
    private Vector3 initialRotation;
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
    }

    private IEnumerator DestroyAfterSound()
    {
      // Esperar un frame para que el sonido pueda iniciarse
      yield return null;

      // Buscar componente original y ejecutar método parar mostrar sus meshes nuevamente
      GetComponentInParent<MainCameraHandler>().lastEventSender.GetComponent<ObjectExaminationHandler>().ShowObjectInScene(true);
      GetComponentInParent<MainCameraHandler>().lastEventSender = null;

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