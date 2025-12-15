using System.Collections;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.UIController;
using System.Collections.Generic;
using TwelveG.PlayerController;
using TwelveG.Utils;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;

namespace TwelveG.GameController
{
    public class BirdsEvent : GameEventBase
    {
        [Header("Event references: ")]
        [Space]
        [SerializeField] private GameObject crashingBirdPrefab;
        [Header("Text event SO")]
        [Space]
        [SerializeField] private List<ObservationTextSO> mainDoorsFallbacksTextsSO;
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;
        [SerializeField] private List<UIOptionsTextSO> playerHelperDataTextSO;

        [Header("Other eventsSO references")]
        [Space]
        [SerializeField] GameEventSO onSpawnVehicle;
        [SerializeField] GameEventSO trashBagCanBePicked;
        [SerializeField] GameEventSO broomCanBePicked;
        [SerializeField] GameEventSO destroyWindowToReplace;
        [SerializeField] GameEventSO zoomBirdIsInteractable;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ BIRDS EVENT NOW -------->");

            // Esto es para actualizar los fallback texts para las Main Doors en particular
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[0]);
            // Actualizar text de ayuda del canvas del menu de pausa al presionar ESC
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

            // Llega al Window to replace que deshabilita el mesh renderer de la ventana, y luego ejecuta
            // el InstantiateZoomBird del WindowToReplaceHandler.
            // zoomBird inicia con su collider interactuable apagado, se prende con el próximo eventoSO.
            destroyWindowToReplace.Raise(this, null);
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            yield return new WaitForSeconds(1f);

            // Parece que algo pasó arriba, mejor reviso qué fue eso.
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[0]
            );

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Uff, voy a tener que limpiar esto si no quiero que vuelvan 
            // y me culpen por esta desastre.
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[1]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventsObservationTextSO[1].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[1]);

            // Se habilita el collider del interactuable de zoomBird
            zoomBirdIsInteractable.Raise(this, null);

            broomCanBePicked.Raise(this, true);

            trashBagCanBePicked.Raise(this, true);

            GameObject crashingBird = Instantiate(crashingBirdPrefab);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            // Y ahora qué m...
            yield return new WaitForSeconds(0.5f);
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[2]
            );

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(1.5f);
            // Mejor tiro esta basura antes que ...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[3]
            );

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[2]);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitForSeconds(1f);

            // A otra cosa.
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[4]
            );

            // Reset manual de textos de Player Data Helper y mainDoorsFallacks a default
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
        }

        public void AllowNextActions(Component sender, object data)
        {
            print(gameObject.name + "recieved event sent by: " + sender.gameObject.name);
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }
    }
}
