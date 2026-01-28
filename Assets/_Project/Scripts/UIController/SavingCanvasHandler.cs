using System.Collections;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.UIController
{
  public class SavingCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField, Range(0f, 5f)] private float delayTime = 1f;
    [SerializeField, Range(1f, 16f)] private float routineDuration = 4f;
    [SerializeField, Range(1, 10)] private int cycles = 4;

    private Canvas savingCanvas;

    private void Awake()
    {
      savingCanvas = GetComponent<Canvas>();
      canvasGroup.alpha = 0f;
    }

    private void Start()
    {
      savingCanvas.enabled = false;
    }

    // Recibe "onCheckpointEventReached" desde EventsHandler.cs
    public void SavingCoroutine(Component sender, object data)
    {
      EventsEnum currentEvent = (EventsEnum)data;

      if (currentEvent == EventsEnum.WakeUp || currentEvent == EventsEnum.WakeUpAtNight) return;

      StartCoroutine(SavingSequence());
    }

    private IEnumerator SavingSequence()
    {
      yield return new WaitForSeconds(delayTime);

      savingCanvas.enabled = true;

      yield return StartCoroutine(UIUtils.BlinkAlphaForDuration(canvasGroup, routineDuration, cycles));

      savingCanvas.enabled = false;
    }
  }
}