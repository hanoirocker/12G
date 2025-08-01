namespace TwelveG.PlayerController
{
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerShortcuts : MonoBehaviour
    {
        [Header("Game Event So's")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onActivateCanvas;
        public GameEventSO onDeactivateCanvas;

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
                print("gameIsPaused: " + PauseMenuCanvasHandler.gameIsPaused);
                if (!PauseMenuCanvasHandler.gameIsPaused)
                {
                    onActivateCanvas.Raise(this, CanvasHandlerType.PauseMenu);
                    playerHandler.DisablePlayerCapsule();
                }
                else
                {
                    onDeactivateCanvas.Raise(this, CanvasHandlerType.PauseMenu);
                    playerHandler.EnablePlayerCapsule();
                }
            }
        }
    }

}