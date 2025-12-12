using TwelveG.GameController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.UIController
{
    public class ResumeButtonHandler : MonoBehaviour
    {
        public GameEventSO onPauseMenuControls;
        public GameEventSO onPlayerControls;

        public void ResumeGameButtonClick()
        {
            onPauseMenuControls.Raise(this, new ActivateCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
        }
    }
}