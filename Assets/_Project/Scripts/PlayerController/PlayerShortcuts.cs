namespace TwelveG.PlayerController
{
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerShortcuts : MonoBehaviour
    {
        [Header("Game Event So's")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onPauseGame;

        public bool playerCanOpenCanvasControls = true;
        public bool playerCanOpenPauseMenu = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H) && playerCanOpenCanvasControls)
            {
                onControlCanvasControls.Raise(this, new AlternateCanvasCurrentState());
            }
            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     Time.timeScale = 1;
            // }
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     Time.timeScale = 2;
            // }
            else if (Input.GetKeyDown(KeyCode.Escape) && playerCanOpenPauseMenu)
            {
                if (!PauseMenuCanvasHandler.gameIsPaused)
                {
                    onPauseGame.Raise(this, true);
                }
                else
                {
                    onPauseGame.Raise(this, false);
                }
            }
        }
    }

}