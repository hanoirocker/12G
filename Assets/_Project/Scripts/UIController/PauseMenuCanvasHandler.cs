namespace TwelveG.UIController
{
  using TwelveG.PlayerController;
  using UnityEngine;

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

        public void ResumeGameOption()
        {
            onPlayerControls.Raise(this, new TogglePlayerCapsule(true));
        }
    }
}