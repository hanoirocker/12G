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
        [Header("Audio Options")]
        [Space]
        [SerializeField][Range(0.1f, 10f)] private float bgMusicFadeOut = 6f;
        [Header("Text event SO")]
        [Space]
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;

        [Header("EventsSO references")]
        [Space]
        [SerializeField] GameEventSO onImageCanvasControls;
        [SerializeField] GameEventSO onObservationCanvasShowText;
        [SerializeField] GameEventSO onVirtualCamerasControl;
        [SerializeField] GameEventSO onPlayerControls;
        [SerializeField] GameEventSO onMainCameraSettings;

        [Header("Other eventsSO references")]
        [Space]
        [SerializeField] GameEventSO onSpawnVehicle;
        [SerializeField] GameEventSO trashBagCanBePicked;
        [SerializeField] GameEventSO broomCanBePicked;
        [SerializeField] GameEventSO updateFallbackTexts;
        [SerializeField] GameEventSO destroyWindowToReplace;
        [SerializeField] GameEventSO zoomBirdIsInteractable;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ BIRDS EVENT NOW -------->");

            // Esto es para actualizar los fallback texts para las Main Doors en particular
            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            // Llega al Window to replace que deshabilita el mesh renderer de la ventana, y luego ejecuta
            // el InstantiateZoomBird del WindowToReplaceHandler.
            // zoomBird inicia con su collider interactuable apagado, se prende con el próximo eventoSO.
            destroyWindowToReplace.Raise(this, null);
            onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, false));
            onPlayerControls.Raise(this, new EnablePlayerControllers(true));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            yield return new WaitForSeconds(1f);

            // Parece que algo pasó arriba, mejor reviso qué fue eso.
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[0]
            );

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Uff, voy a tener que limpiar esto si no quiero que vuelvan 
            // y me culpen por esta desastre.
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[1]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventsObservationTextSO[1].observationTextsStructure[0].observationText
            ));

            // Se habilita el collider del interactuable de zoomBird
            zoomBirdIsInteractable.Raise(this, null);

            broomCanBePicked.Raise(this, true);

            trashBagCanBePicked.Raise(this, true);

            GameObject crashingBird = Instantiate(crashingBirdPrefab);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            // Y ahora qué m...
            yield return new WaitForSeconds(0.5f);
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[2]
            );

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(1.5f);
            // Mejor tiro esta basura antes que ...
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[3]
            );

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitForSeconds(1f);
            AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnActiveSourceByType(AudioPoolType.BGMusic);

            // Detiene "Haunting Sound" clip iniciado en Wake Up Event
            if (bgMusicSource != null)
            {
                StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(bgMusicSource, 3f));
            }

            // A otra cosa.
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[4]
            );
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
