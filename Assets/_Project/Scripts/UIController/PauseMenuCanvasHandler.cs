using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
    public class PauseMenuCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<UpdateTextHandler> textHandlers;

        [Header("Testing")]
        public static bool gameIsPaused;

        private void OnEnable()
        {
            UpdateUITextOptions();
            AudioManager.Instance.AudioUIHandler.PlayPauseMenuSound();
            SetPauseGameSettings();
        }

        private void SetPauseGameSettings()
        {
            Time.timeScale = 0f;
            gameIsPaused = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            AudioManager.Instance.AudioUIHandler.PlayPauseMenuSound();
            SetResumeGameSettings();
        }

        public void UpdateUITextOptions()
        {
            foreach (UpdateTextHandler textHandler in textHandlers)
            {
                textHandler.UpdateText(Localization.LocalizationManager.Instance.GetCurrentLanguageCode());
            }
        }

        public void OnResumeGameButtonPressed()
        {
            Debug.LogWarning($"[PauseMenuCanvasHandler]: Resuming game");
            GameEvents.Common.onPauseGame.Raise(this, false);
        }

        public void OnSaveGameButtonPressed()
        {
            Debug.LogWarning($"[PauseMenuCanvasHandler]: Saving game data");
            DataPersistenceManager.Instance.SavePersistenceData();
            GameEvents.Common.onPauseGame.Raise(this, false);
        }

        public void OnQuitGameButtonPressed()
        {
            Debug.LogWarning($"[MenuCanvasHandler]: Quitting game");
            Application.Quit();
        }

        // Settings se llama desde UIManager, ya que debe detectar al que lo llamó

        private void SetResumeGameSettings()
        {
            Time.timeScale = 1f;
            gameIsPaused = false;

            // Ocultar el cursor y bloquearlo en el centro de la pantalla
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}