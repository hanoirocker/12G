namespace TwelveG.PlayerController
{
    using TwelveG.UIManagement;
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
                onControlCanvasControls.Raise(this, "ToogleControlCanvas");
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!PauseMenuCanvasHandler.gameIsPaused)
                {
                    onPauseMenuToogle.Raise(this, "ActivatePauseMenu");
                    playerHandler.DisablePlayerCapsule();
                }
                else
                {
                    onPauseMenuToogle.Raise(this, "DeactivatePauseMenu");
                    playerHandler.EnablePlayerCapsule();
                }
            }
        }
    }

}