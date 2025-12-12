using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class PizzaTimeEvent : GameEventBase
    {
        [Header("Event options")]
        [Space]
        [TextArea(5, 50)]
        [SerializeField, Range(1, 10)] private int initialTime = 4;

        [Header("Audio settings")]
        [Space]
        [SerializeField] private AudioClip chairMovingSound = null;
        [SerializeField][Range(0f, 1f)] private float chairMovingSoundVolume = 0.8f;

        [Header("Text event SO")]
        [Space]
        [SerializeField] private ObservationTextSO[] mainDoorsFallbacksTextsSO;
        [SerializeField] private List<UIOptionsTextSO> playerHelperDataTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;
        [SerializeField] private EventsControlCanvasInteractionTextSO eventsControlCanvasInteractionTextSO_eating;

        [Header("Other eventsSO references")]
        [Space]
        [SerializeField] private GameEventSO plateCanBePicked;
        [SerializeField] private GameEventSO pizzaCanBePicked;
        [SerializeField] private GameEventSO updateFallbackTexts;
        [SerializeField] private GameEventSO onSpawnVehicle;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ PIZZA TIME EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[0]
            );

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventsObservationTextSO[0].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[0]);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

            // Esto notifica al plato para que haga AllowItemToBePicked = true;
            pizzaCanBePicked.Raise(this, true);
            plateCanBePicked.Raise(this, true);

            // Unity Event (Pizza Handler - onPizzaPickedUp):
            // Luego de tomar la porción de pizza menciona que hay que
            // Calentarla
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[1]
            );

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[1]);

            // Unity Event (MicrowaveHandler - pizzaHeatingFinished):
            // Se muestra el diálogo luego de calentar pizza
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitForSeconds(0.5f);
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[2]
            );

            // Unity Event (SitAndEat - sitAndEatPizza):
            // Al interactuar con SitAndEat (mesa de la cocina) se deshabilitan controles pples 
            // del jugador y se pasa a la VC KitchenDeskVC
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));


            if (chairMovingSound)
            {
                AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
                audioSource.volume = chairMovingSoundVolume;
                audioSource.PlayOneShot(chairMovingSound);
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
            else
            {
                // Espera lo que tarda por defecto el FadeOutImage
                yield return new WaitForSeconds(1);
            }

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDesk, true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            yield return new WaitForSeconds(1f);

            GameEvents.Common.onControlCanvasSetInteractionOptions.Raise(
                this,
                eventsControlCanvasInteractionTextSO_eating
            );

            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            // Unity Event (PizzaSliceHandler - instantiatePoliceCar)
            // Avisa que va por la segunda mordida y se debe instanciar el auto de policia
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.RegularPoliceCar);

            // Espera mas o menos un tiempo adecuado para cuando el auto de la policia
            // ya haya pasado y comenta al respecto.
            yield return new WaitForSeconds(6f);
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[3]
            );

            // Unity Event (PizzaSliceHandler - finishedEatingPizza):
            // Se invoca finishedEatingPizza al final de comer la pizza para que el jugador vuelva
            // a estar disponible.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onControlCanvasControls.Raise(this, new ResetControlCanvasSpecificOptions());
            yield return new WaitForSeconds(3.5f);
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            // LEVANTARSE [E]
            GameEvents.Common.onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            // Espera hasta que el jugador presione nuevamente la E
            // para levantarse de la silla.
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            GameEvents.Common.onInteractionCanvasControls.Raise(this, new HideText());
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 2f));
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);
            yield return new WaitForSeconds(2f);
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDesk, false));
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 2f));
            yield return new WaitForSeconds(1f);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }
    }
}