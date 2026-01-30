using System.Collections;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class LockedUpEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 0;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO[] observationTextSOs;
        [SerializeField] private DialogSO[] dialogSOs;

        private bool walkieTalkiePickedUp = false;
        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            // DESACTIVAR SPRINT DEL JUGADOR DURANTE PRIMEROS EVENTOS
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerSprint(false));
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 1f));

            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            UIManager.Instance.ControlCanvasHandler.ToggleControlCanvas(false);
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));

            yield return new WaitForSeconds(3f);
            // Qué mierda hago en mi habitación? Mi walkie talkie ...
            if (!walkieTalkiePickedUp)
            {
                UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                    observationTextSOs[0]
                );
            }

            // Recibe "onWalkieTalkiePickedUp" para permitir la siguiente acción
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);
            // Sonidos extraños, y Simon intentando hablar.
            GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

            // "onConversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(10f);
            // "Necesito hablarte ..."
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                observationTextSOs[1]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                observationTextSOs[1].observationTextsStructure[0].observationText + 3f
            ));

            // Intenta comunicarse con Micaela
            GameEvents.Common.onStartDialog.Raise(this, dialogSOs[1]);

            // "onConversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(2f);

            // Se desbloquea sola la puerta de su pieza
            yield return StartCoroutine(PlayerHouseHandler.Instance.
                GetStoredObjectByID("Players Door Lock").
                GetComponent<DownstairsOfficeDoorHandler>().UnlockDoorByEventCoroutine());

            yield return new WaitForSeconds(3f);
        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

        public void OnWalkieTalkiePickedUp(Component sender, object data)
        {
            walkieTalkiePickedUp = true;
            allowNextAction = true;
        }
    }
}