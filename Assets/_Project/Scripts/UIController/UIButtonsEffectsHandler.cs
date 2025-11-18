namespace TwelveG.UIController
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIButtonsEffectsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Header("Highlight Background")]
        [SerializeField] private GameObject highlightBG;

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
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (highlightBG != null)
                highlightBG.SetActive(false);
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

    }
}