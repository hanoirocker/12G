using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.VFXController;
using UnityEngine;

namespace TwelveG.GameController
{
    public class HeadachesEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 2;
        [Tooltip("Duración para cerrar los ojos")]
        [SerializeField] private float eyesCloseDuration = 6f;

        [Header("Text event SO")]
        [SerializeField] private DialogSO dialogOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.PoliceCarCrash);

            // "Police Car Crashed - Spot" dispara el evento policeCarSpotted al ser chekeado por Simon
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(2f);
            // Conversación que inicia Mica desesperada por la situación
            GameEvents.Common.onStartDialog.Raise(this, dialogOs);

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
            float currentInGameVolume = AudioManager.Instance.GetCurrentChannelVolume("inGameVol");
            AudioManager.Instance.FaderHandler.FadeAudioGroup(
                AudioGroup.inGameVol,
                currentInGameVolume,
                0,
                eyesCloseDuration
            );

            yield return new WaitForSeconds(eyesCloseDuration);

            // Detener todas las audio sources de la pool inGameVol
            AudioManager.Instance.PoolsHandler.StopActiveSourcesOnMixChannel(AudioMixChannel.InGame);

            yield return new WaitForFixedUpdate();
            AudioManager.Instance.FaderHandler.FadeAudioGroup(
                AudioGroup.inGameVol,
                0,
                currentInGameVolume,
                3f
            );

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

        public void StopEvent()
        {
            StopAllCoroutines();
        }
    }
}
