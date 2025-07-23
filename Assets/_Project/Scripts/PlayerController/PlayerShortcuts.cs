namespace TwelveG.PlayerController
{
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerShortcuts : MonoBehaviour
    {
        public GameEventSO onPauseMenuToogle;
        public GameEventSO onControlCanvasControls;

        private PlayerHandler playerHandler;

        private void Awake()
        {
            playerHandler = GetComponent<PlayerHandler>();
        }

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
                    onPauseMenuToogle.Raise(this, new ActivateCanvas(true));
                    playerHandler.DisablePlayerCapsule();
                }
                else
                {
                    onPauseMenuToogle.Raise(this, new ActivateCanvas(false));
                    playerHandler.EnablePlayerCapsule();
                }
            }
        }
    }

}