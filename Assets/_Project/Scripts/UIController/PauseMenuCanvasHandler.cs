namespace TwelveG.UIController
{
    using TwelveG.PlayerController;
    using TwelveG.SaveSystem;
    using UnityEngine;
    using UnityEngine.UI;

    public class PauseMenuCanvasHandler : MonoBehaviour
    {
        public static bool gameIsPaused;

        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onPlayerControls;

        private void OnEnable()
        {
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
            SetResumeGameSettings();
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
                    onPlayerControls.Raise(this, new TogglePlayerCapsule(true));
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