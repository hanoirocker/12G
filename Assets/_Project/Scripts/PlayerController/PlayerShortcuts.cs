using TwelveG.GameController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.PlayerController
{
    public class PlayerShortcuts : MonoBehaviour
    {
        public bool playerCanOpenCanvasControls = true;
        public bool playerCanOpenPauseMenu = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H) && playerCanOpenCanvasControls)
            {
                GameEvents.Common.onControlCanvasControls.Raise(this, new AlternateCanvasCurrentState());
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && playerCanOpenPauseMenu)
            {
                if (!PauseMenuCanvasHandler.gameIsPaused)
                {
                    GameEvents.Common.onPauseGame.Raise(this, true);
                }
                else
                {
                    GameEvents.Common.onPauseGame.Raise(this, false);
                }
            }
        }
    }

}