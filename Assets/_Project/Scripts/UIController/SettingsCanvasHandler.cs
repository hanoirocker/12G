namespace TwelveG.GameController
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class SettingsCanvasHandler : MonoBehaviour
    {
        [Header("Basic Options Refs")]
        [SerializeField] private GameObject generalOptions;
        [SerializeField] private GameObject audioOptions;
        [SerializeField] private GameObject videoOptions;
        [SerializeField] private GameObject defaultOptions;
        [SerializeField] private Button defaultOptionsButton;

        [Header("Graphics Settings Refs")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Graphics Default Values")]
        // TODO: trabajar en función reset to default values al presionar Return btn.

        private int _qualityLevel;
        private bool _isFullscreen;
        private float _brightnessLevel;
        private Resolution[] resolutions;
        private GameObject lastActiveOptions;

        private void OnEnable()
        {
            FillResolutionDropdown();

            // Por defecto abrir el panel de opciones generales
            defaultOptions.SetActive(true);
            lastActiveOptions = defaultOptions;

            // Al iniciar el Settings Canvas, selecciona y presiona la opción por defecto
            // (General Options panel gameobject)
            EventSystem.current.SetSelectedGameObject(defaultOptionsButton.gameObject);
            defaultOptionsButton.onClick.Invoke();
        }

        private void OnDisable()
        {
            lastActiveOptions = defaultOptions;
        }

        // Encontrar todas las configuraciones de resolución y populate
        // la lista del dropdown de resoluciones.
        private void FillResolutionDropdown()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetLanguage()
        {
            // TODO:
            // llamar función general de UIManger para actualizar TODOS los textos
            // llamar a localization para setear el nuevo lenguage.
            // Guardar el nuevo lenguage
        }

        public void ApplyGraphicChanges()
        {
            // TODO: Aplicar y guardar los cambios
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        // Callback desde Quality Dropdown. No aplica directamente sino que 
        // almacena el valor para setearlo al aceptar el confirmation panel
        public void SetQuality(int qualityIndex)
        {
            _qualityLevel = qualityIndex;
            // TODO: aplicar y guardar los cambios
        }

        // Callback desde Brightness Slider para actualizar automáticamente
        // el valor de brillo
        public void SetBrightness(float brightnessLevel)
        {
            _brightnessLevel = brightnessLevel;
            // TODO: aplicar y guardar los cambios
            // Camara? PostProcessing? wtf
        }

        // Callback desde Full Screen Toggle. No aplica directamente sino que 
        // almacena el valor para setearlo al aceptar el confirmation panel
        public void SetFullScreen(bool isFullscreen)
        {
            _isFullscreen = isFullscreen;
            // TODO: aplicar y guardar los cambios
        }

        public void OnSettingsButtonClicked(Button clickedButton)
        {
            string buttonName = clickedButton.name;
            lastActiveOptions.SetActive(false);

            switch (buttonName)
            {
                case "General Btn":
                    lastActiveOptions = generalOptions;
                    break;
                case "Audio Btn":
                    lastActiveOptions = audioOptions;
                    break;
                case "Video Btn":
                    lastActiveOptions = videoOptions;
                    break;
                default:
                    Debug.LogError($"[SettingsCanvasHandler]: clickedButton not recognized");
                    break;
            }
        }
    }
}