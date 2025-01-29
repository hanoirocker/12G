namespace TwelveG.UIManagement
{
    using UnityEngine;

    public class ResumeButtonHandler : MonoBehaviour
    {
        public GameEventSO onPauseMenuControls;
        public GameEventSO onPlayerControls;

        public void ResumeGameButtonClick()
        {
            onPauseMenuControls.Raise(this, "DeactivatePauseMenu");
            onPlayerControls.Raise(this, "EnablePlayerCapsule");
        }
    }
}