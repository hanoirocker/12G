using System;
using System.Collections;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.UIController
{
    public enum InteractionObjectType
    {
        None,
        TV,
        RemoteControl,
        WalkieTalkie,
        Flashlight,
    }

    public class ControlCanvasHandler : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject TVOptions;
        [SerializeField] private GameObject RCOptions;
        [SerializeField] private GameObject WTOptions;
        [SerializeField] private GameObject FlashlightOptions;

        [Space(10)]
        [Header("Settings")]
        [SerializeField, Range(0f, 3f)] private float fadeDuration = 3f;

        private Canvas controlCanvas;

        void Awake()
        {
            controlCanvas = GetComponent<Canvas>();
            controlCanvas.enabled = false;
        }

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
        }

        // Escucha el evento onControlCanvasSetInteractionOptions llamado desde 
        // PlayerItemBase y PlayerInventory para mostrar/ocultar las opciones
        // específicas de cada objeto de interacción.
        public void SetInteractionSpecificOptions(InteractionObjectType interactionObjectType, bool enabled)
        {
            switch (interactionObjectType)
            {
                case InteractionObjectType.TV:
                    TVOptions.SetActive(enabled);
                    break;
                case InteractionObjectType.RemoteControl:
                    RCOptions.SetActive(enabled);
                    break;
                case InteractionObjectType.WalkieTalkie:
                    WTOptions.SetActive(enabled);
                    break;
                case InteractionObjectType.Flashlight:
                    FlashlightOptions.SetActive(enabled);
                    break;
                default:
                    Debug.LogWarning($"[ControlCanvasHandler] Unknown InteractionObjectType received: {interactionObjectType}");
                    return;
            }
        }

        // Llamar a cada TextMeshProUGUI anidado para actualizar sus textos
        // en relación a sus propios assets SO
        public void UpdateCanvasTextOnLanguageChanged()
        {
            foreach (UpdateTextHandler updateTextHandler in GetComponentsInChildren<UpdateTextHandler>())
            {
                updateTextHandler.UpdateText(LocalizationManager.Instance.GetCurrentLanguageCode());
            }
        }
        
        public void AlternateControlCanvas()
        {
            StartCoroutine(ToggleControlCanvasCoroutine(!controlCanvas.enabled));
        }

        // Ejecutado cuando recibe onEnablePlayerItem (usado para mostrar el canvas al encontrar
        // la linterna o el walkie talkie por primera vez)
        public void ToggleControlCanvas(bool enable)
        {
            StartCoroutine(ToggleControlCanvasCoroutine(enable));
        }

        private IEnumerator ToggleControlCanvasCoroutine(bool enableCanvas)
        {
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            if (enableCanvas)
            {
                controlCanvas.enabled = true;
            }

            float elapsed = 0f;
            float target = enableCanvas ? 1f : 0f;
            float initialAlpha = canvasGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(initialAlpha, target, elapsed / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }

            canvasGroup.alpha = target;

            if (!enableCanvas)
            {
                controlCanvas.enabled = false;
            }

            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
        }
    }
}