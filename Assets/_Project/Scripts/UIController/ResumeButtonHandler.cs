namespace TwelveG.UIController
{
  using TwelveG.PlayerController;
  using UnityEngine;

    public class ResumeButtonHandler : MonoBehaviour
    {
        public GameEventSO onPauseMenuControls;
        public GameEventSO onPlayerControls;

        public void ResumeGameButtonClick()
        {
            onPauseMenuControls.Raise(this, new ActivateCanvas(false));
            onPlayerControls.Raise(this, new TogglePlayerCapsule(true));
        }
    }
}