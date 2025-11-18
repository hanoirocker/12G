using System.Collections;
using UnityEngine;
namespace TwelveG.UIController
{
  public class LoadingSceneCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private GameObject continueText;

    [Header("Transition Settings")]

    [SerializeField, Range(0.25f, 2f)] private float blinkSpeed = 2f;
    [SerializeField, Range(0.25f, 1f)] private float blinkTime = 0.25f;
    [SerializeField, Range(0f, 3f)] private float delayBeforeText = 0.5f;

    private bool keepBlinking = true;
    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      StartCoroutine(BlinkingSequence());
    }

    private IEnumerator BlinkingSequence()
    {
      while (keepBlinking)
      {
        blinkTime += Time.deltaTime * blinkSpeed;
        logoCanvasGroup.alpha = Mathf.PingPong(blinkTime, 1f);
        yield return null;
      }

      logoCanvasGroup.alpha = 1f;

      yield return new WaitForSeconds(delayBeforeText);

      Debug.Log("Scene loaded. Waiting for key press...");
    }

    private void OnDisable()
    {
      continueText.SetActive(false);
    }

    public void SceneLoaded()
    {
      keepBlinking = false;
      if (!continueText.activeSelf)
      {
        continueText.SetActive(true);
      }
      keepBlinking = true;
    }

    public void OnAnyKeyPressed()
    {
      if (continueText.activeSelf)
      {
        continueText.SetActive(false);
      }
    }
  }
}