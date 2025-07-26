namespace TwelveG.GameController
{
    using UnityEngine;
  using UnityEngine.EventSystems;
  using UnityEngine.UI;

    public class SettingsCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject generalOptions;
        [SerializeField] private GameObject audioOptions;
        [SerializeField] private GameObject videoOptions;
        [SerializeField] private GameObject defaultOptions;
        [SerializeField] private Button defaultOptionsButton;

        private GameObject lastActiveOptions;

        private void OnEnable()
        {
            // Por defecto abrir el panel de opciones generales
            defaultOptions.SetActive(true);
            lastActiveOptions = defaultOptions;

            // Opcional: Ejecuta el onClick
            EventSystem.current.SetSelectedGameObject(defaultOptionsButton.gameObject);
            defaultOptionsButton.onClick.Invoke();
        }

        public void OnSettingsButtonClicked(Button clickedButton)
        {
            string buttonName = clickedButton.name;
            lastActiveOptions.SetActive(false);

            switch (buttonName)
            {
                case "General Button":
                    lastActiveOptions = generalOptions;
                    break;
                case "Audio Button":
                    lastActiveOptions = audioOptions;
                    break;
                case "Video Button":
                    lastActiveOptions = videoOptions;
                    break;
                case "Return Button":
                    lastActiveOptions = defaultOptions;
                    break;
                default:
                    Debug.LogError($"[SettingsCanvasHandler]: clickedButton not recognized");
                    break;
            }
        }
    }
}