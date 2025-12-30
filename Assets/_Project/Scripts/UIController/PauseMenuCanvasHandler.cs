using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
    public class PauseMenuCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioClip inGameMenuClip;
        [SerializeField] private List<UpdateTextHandler> textHandlers;

        [Header("Testing")]
        public static bool gameIsPaused;

        private AudioSource pauseMenuSource;

        private void OnEnable()
        {
            UpdateUITextOptions();
            PlayPauseMenuSound();
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
            PlayPauseMenuSound();
            SetResumeGameSettings();
        }

        public void UpdateUITextOptions()
        {
            foreach (UpdateTextHandler textHandler in textHandlers)
            {
                textHandler.UpdateText(Localization.LocalizationManager.Instance.GetCurrentLanguageCode());
            }
        }

        private void PlayPauseMenuSound()
        {
            if (pauseMenuSource == null)
            {
                pauseMenuSource = AudioManager.Instance?.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.UI);
            }

            if (inGameMenuClip == null) { return; }

            if (pauseMenuSource)
            {
                if (pauseMenuSource.isPlaying) { pauseMenuSource.Stop(); }
                pauseMenuSource.PlayOneShot(inGameMenuClip);
            }
        }

        private void SetResumeGameSettings()
        {
            Time.timeScale = 1f;
            gameIsPaused = false;

            // Ocultar el cursor y bloquearlo en el centro de la pantalla
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnPauseMenuButtonClicked(Button btn)
        {
            switch (btn.name)
            {
                case "Settings Btn":
                    // Load Settings canvas
                    print("TODO: Open Setting Canvas now!");
                    break;
                case "Save Btn":
                    print("Saving data!");
                    DataPersistenceManager.Instance.SavePersistenceData();
                    break;
                case "Return Btn":
                    GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
                    break;
                case "Quit Btn":
                    print("Quitting game!");
                    Application.Quit();
                    break;
                default:
                    Debug.LogError($"[PauseMenuCanvasHandler]: clickedButton not recognized");
                    break;
            }
        }
    }
}