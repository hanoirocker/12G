using System.Collections;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.GameController
{
    public class LockedUpEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO[] observationTextSOs;

        [Space]
        [Header("Audio Options")]
        // [SerializeField] private AudioClip nightStandUpClip;

        [Space]
        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onPlayerDoorUnlock;

        private bool flashLightWasPickedUp = false;
        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            // DESACTIVAR SPRINT DEL JUGADOR DURANTE PRIMEROS EVENTOS
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerSprint(false));
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));

            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));

            yield return new WaitForSeconds(10f);
            // Supongo que una linterna me vendria bien ..
            if (!flashLightWasPickedUp)
            {
                GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSOs[0]);
            }

            // Recibe "onFlashlightPickedUp" para permitir la siguiente acción
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Ruido de desbloqueo de la puerta
            onPlayerDoorUnlock.Raise(this, null);

            yield return new WaitForSeconds(2f);
            // Ya era hora ..
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSOs[1]);
        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

        public void OnFlashLightPickedUp(Component sender, object data)
        {
            flashLightWasPickedUp = true;
            allowNextAction = true;
        }
    }
}