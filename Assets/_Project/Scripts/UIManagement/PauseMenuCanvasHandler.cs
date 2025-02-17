namespace TwelveG.UIManagement
{
    using UnityEngine;

    public class PauseMenuCanvasHandler : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuPanel;

        public static bool gameIsPaused = false;

        private void Start()
        {
            gameIsPaused = false;
        }

        private void ResumeGame()
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            gameIsPaused = false;

            // Ocultar el cursor y bloquearlo en el centro de la pantalla
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public bool isGamePaused()
        {
            return gameIsPaused;
        }

        private void PauseGame()
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
            gameIsPaused = true;

            // Mostrar el cursor y desbloquearlo
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void PauseMenuCanvasControl(Component sender, object data)
        {
            if(!gameIsPaused && (string)data == "ActivatePauseMenu")
            {
                PauseGame();
            }
            else if(gameIsPaused && (string)data == "DeactivatePauseMenu")
            {
                ResumeGame();
            }
        }
    }
}