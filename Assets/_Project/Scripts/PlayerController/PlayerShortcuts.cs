using TwelveG.AudioController;
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
                UIManager.Instance.ControlCanvasHandler.AlternateControlCanvas();
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
            else if(Input.GetKeyDown(KeyCode.U))
            {
                GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.CloseThunder);
            }
            else if(Input.GetKeyDown(KeyCode.J))
            {
                GameEvents.Common.onCancelCurrentDialog.Raise(this, null);
            }
        }
    }

}