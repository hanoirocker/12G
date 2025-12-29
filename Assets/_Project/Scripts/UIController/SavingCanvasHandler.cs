using System.Collections;
using UnityEngine;

namespace TwelveG.UIController
{
  public class SavingCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
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

    public void SavingCoroutine()
    {
      StartCoroutine(SavingSequence());
    }

    private IEnumerator SavingSequence()
    {
      yield return new WaitForSeconds(delayTime);

      savingCanvas.enabled = true;

      float elapsed = 0f;
      float cycleTime = routineDuration / cycles;

      while(elapsed < routineDuration)
      {
        elapsed += Time.deltaTime;
        canvasGroup.alpha = Mathf.PingPong(elapsed, cycleTime) / cycleTime;
        yield return null;
      }
  
      savingCanvas.enabled = false;
    }
  }
}