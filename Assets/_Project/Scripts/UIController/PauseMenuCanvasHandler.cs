namespace TwelveG.UIController
{
    using TwelveG.AudioController;
    using TwelveG.PlayerController;
    using TwelveG.SaveSystem;
    using UnityEngine;
    using UnityEngine.UI;

    public class PauseMenuCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioClip inGameMenuClip;

        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onPlayerControls;

        [Header("Testing")]
        public static bool gameIsPaused;

        private AudioSource inGameAudioSource;

        private void Awake()
        {
            inGameAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.UI);
        }

        private void OnEnable()
        {
            PlayInGameMenuSound();
            SetPauseGameSettings();
        }

        private void SetPauseGameSettings()
        {
            Time.timeScale = 0f;
            gameIsPaused = true;

            // Mostrar el cursor y desbloquearlo
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            PlayInGameMenuSound();
            SetResumeGameSettings();
        }

        private void PlayInGameMenuSound()
        {
            if (inGameAudioSource == null) { return; }

            if (inGameMenuClip == null)
            {
                Debug.Log("[MenuCanvasHandler]: gameMenuSound not assigned!");
                return;
            }

            if (inGameAudioSource.isPlaying) { inGameAudioSource.Stop(); }
            inGameAudioSource.PlayOneShot(inGameMenuClip);
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
                    onPlayerControls.Raise(this, new EnablePlayerControllers(true));
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