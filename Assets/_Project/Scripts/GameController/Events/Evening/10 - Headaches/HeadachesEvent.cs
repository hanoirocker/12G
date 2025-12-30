using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using TwelveG.VFXController;
using UnityEngine;

namespace TwelveG.GameController
{
    public class HeadachesEvent : GameEventBase
    {
        [SerializeField] private GameEventListener onConversationHasEndedListener;
        [Space(10)]
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 2;
        [Tooltip("Duración para cerrar los ojos")]
        [SerializeField, Range(1f, 50f)] private float timeUntilPoliceCarCrash = 20f;
        [SerializeField] private float eyesCloseDuration = 6f;

        [Space(10)]
        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO[] eventObservationsTextsSOs;
        [SerializeField] private DialogSO[] dialogOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            // TODO: borrar (solo de prueba)
            // GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            // Debo contactar a Mica urgente...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[0].observationTextsStructure[0].observationText
            ));

            // Simon intenta contactarse con Mica nuevamente luego de haber escuchado
            // la conversación policial en el evento anterior (Noises). MICA NO CONTESTA.
            GameEvents.Common.onStartDialog.Raise(this, dialogOs[0]);

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
            // Apagar el listener para que el último dialogo no resetee el allowNextAction
            onConversationHasEndedListener.enabled = false;

            yield return new WaitForSeconds(timeUntilPoliceCarCrash);

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.PoliceCarCrash);

            // "Police Car Crashed - Spot" dispara el evento policeCarSpotted al ser chekeado por Simon
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(2f);
            // Conversación que inicia Mica desesperada por la situación
            GameEvents.Common.onStartDialog.Raise(this, dialogOs[1]);

            // Durante el dialogo con Mica se dispara el evento "onHeadacheMaxIntensity"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
            VFXManager.Instance.SetElectricFeelIntensity(1f);

            // Esperar efecto de Depth of Field, Vignette y filtro pasa graves
            yield return new WaitForSeconds(15f);

            VFXManager.Instance.TriggerProceduralFaint();
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            // A esta altura Simon ya debería haber caido al suelo por el dolor de cabeza
            // y el image canvas debería estar negro. Todo esto es manejado por el VFXManager?

            // ProceduralFaint terminó al cerar los ojos, levanta "onProceduralFaintFinished"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, eyesCloseDuration));

            // Fade out del canal inGameVol
            AudioManager.Instance.FaderHandler.FadeAudioGroup(
                AudioGroup.inGameVol,
                AudioManager.Instance.GetCurrentChannelVolume("inGameVol"),
                0,
                eyesCloseDuration
            );

            yield return new WaitForSeconds(eyesCloseDuration);

            // Detener todas las audio sources de la pool inGameVol
            AudioManager.Instance.PoolsHandler.StopActiveSourcesOnMixChannel(AudioMixChannel.InGame);
        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

        public void StopEvent()
        {
            StopAllCoroutines();
        }
    }
}
