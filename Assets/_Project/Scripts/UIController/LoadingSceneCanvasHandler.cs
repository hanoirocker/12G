using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TwelveG.UIController
{
  public class LoadingSceneCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private GameObject continueText;
    [SerializeField] private TextMeshProUGUI continueTextComponent;

    [Header("Transition Settings")]
    [SerializeField, Range(0.25f, 2f)] private float blinkSpeed = 2f;
    [SerializeField, Range(0f, 3f)] private float delayBeforeText = 0.5f;

    private Coroutine blinkLogoRoutine;
    private Coroutine blinkTextRoutine;
    private CanvasGroup continueTextCG;

    private void Awake()
    {
      if (continueText != null)
      {
        continueTextCG = continueText.GetComponent<CanvasGroup>();
      }
    }

    private void OnEnable()
    {
      blinkLogoRoutine = StartCoroutine(UIUtils.BlinkAlphaContinuous(logoCanvasGroup, blinkSpeed));
      UIManager.Instance.UIFormatter.AssignFontByType(UIFormatingType.PlayerInteractionText, continueTextComponent);
    }

    private void OnDisable()
    {
      StopAllCoroutines();
      if (continueText != null) continueText.SetActive(false);
    }

    // Llamado cuando la carga termina
    public void SceneLoaded()
    {
      if (blinkLogoRoutine != null) StopCoroutine(blinkLogoRoutine);

      logoCanvasGroup.alpha = 1f;

      StartCoroutine(ShowTextSequence());
    }

    private IEnumerator ShowTextSequence()
    {
      yield return new WaitForSeconds(delayBeforeText);

      Debug.Log("Scene loaded. Waiting for key press...");

      if (continueText != null && !continueText.activeSelf)
      {
        continueText.SetActive(true);
        // Iniciamos el parpadeo del texto
        blinkTextRoutine = StartCoroutine(UIUtils.BlinkAlphaContinuous(continueTextCG, blinkSpeed));
      }
    }

    // Recibe "onAnyKeyPressed" desde SceneLoaderHandler.cs que envía con la duración
    // del fade out de la música para usar como fade del texto y logo
    public void OnAnyKeyPressed(Component component, object data)
    {
      float fadeDuration = (float)data;

      if (continueText.activeSelf)
      {
        if (blinkTextRoutine != null) StopCoroutine(blinkTextRoutine);
        VanishTextAndLogoRoutine(fadeDuration);
      }
    }

    private void VanishTextAndLogoRoutine(float fadeDuration)
    {
      StartCoroutine(UIUtils.FadeCanvasGroup(continueTextCG, 1f, 0f, fadeDuration));
      StartCoroutine(UIUtils.FadeCanvasGroup(logoCanvasGroup, 1f, 0f, fadeDuration));
    }
  }
}