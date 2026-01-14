namespace TwelveG.UIController
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIButtonsEffectsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Header("Highlight Background")]
        [SerializeField] private GameObject highlightBG;
        [SerializeField] private CanvasGroup imageCanvasGroup;
        [SerializeField, Range(0f, 1f)] private float transitionDuration = 0.5f;

        private void Awake()
        {
            if (highlightBG != null)
                highlightBG.SetActive(false);
        }

        // Mouse hover
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlightBG != null)
                highlightBG.SetActive(true);
            StartCoroutine(FadeCanvasGroup(imageCanvasGroup, 0f, 1f, transitionDuration));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (highlightBG != null)
                highlightBG.SetActive(false);
            StartCoroutine(FadeCanvasGroup(imageCanvasGroup, imageCanvasGroup.alpha, 1f, transitionDuration));
        }

        // Gamepad / teclado
        public void OnSelect(BaseEventData eventData)
        {
            if (highlightBG != null)
                highlightBG.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (highlightBG != null)
                highlightBG.SetActive(false);
        }

        protected IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                group.alpha = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            group.alpha = to;
        }

    }
}