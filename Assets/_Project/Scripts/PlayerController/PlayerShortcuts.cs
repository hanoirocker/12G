namespace TwelveG.PlayerController
{
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerShortcuts : MonoBehaviour
    {
        [Header("Game Event So's")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onPauseGame;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                onControlCanvasControls.Raise(this, new AlternateCanvasCurrentState());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
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