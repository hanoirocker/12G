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

        [Header("Text event SO")]
        [Space]
        [SerializeField] private ObservationTextSO[] mainDoorsFallbacksTextsSO;
        [SerializeField] private List<UIOptionsTextSO> playerHelperDataTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;

        [Header("Other eventsSO references")]
        [Space]
        [SerializeField] private GameEventSO plateCanBePicked;
        [SerializeField] private GameEventSO pizzaCanBePicked;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
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

            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[1]
            );

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[1]);

            // Unity Event (MicrowaveHandler - PizzaHeatingFinished):
            // Se muestra el diálogo luego de calentar pizza
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[2]
            );

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[2]);

            // Habilitar collider de la mesa de la cocina para poder sentarse a comer la pizza
            PlayerHouseHandler.Instance.GetStoredObjectByID("Dinner Table Interactable").GetComponent<Collider>().enabled = true;

            // Unity Event (SitAndEat - sitAndEatPizza):
            // Al interactuar con SitAndEat (mesa de la cocina) se deshabilitan controles pples 
            // del jugador y se pasa a la VC KitchenDeskVC
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableInteractionModules(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 1f));

            yield return StartCoroutine(
                AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>().
                PlayPlayerSound(PlayerSoundsType.ChairMoving)
            );

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDesk, true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 1f));

            // Unity Event (PizzaSliceHandler - instantiatePoliceCar)
            // Avisa que va por la segunda mordida y se debe instanciar el auto de policia
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.RegularPoliceCar);

            // Espera mas o menos un tiempo adecuado para cuando el auto de la policia
            // ya haya pasado y comenta al respecto.
            yield return new WaitForSeconds(6f);
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[3]
            );
            // Unity Event (PizzaSliceHandler - finishedEatingPizza):
            // Se invoca finishedEatingPizza al final de comer la pizza para que el jugador vuelva
            // a estar disponible.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(3.5f);
            UIManager.Instance.ControlCanvasHandler.ToggleControlCanvas(false);
            // LEVANTARSE [E]
            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);

            // Espera hasta que el jugador presione nuevamente la E
            // para levantarse de la silla.
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);
            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 2f));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDesk, false));
            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 1f));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableInteractionModules(true));

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
        }

        // Recibe desde el MicrowaveHandler "onSpecificHeatingTimeReached"
        public void OnSpecificHeatingTimeReached(Component sender, object data)
        {
            PlayerHandler playerHandler = PlayerHandler.Instance;
            PlayerSoundsHandler playerSoundsHandler = playerHandler.GetComponentInChildren<PlayerSoundsHandler>();

            if (playerHandler == null) return;

            HouseArea currentArea = playerHandler.GetCurrentHouseArea();

            if (currentArea == HouseArea.Kitchen || currentArea == HouseArea.LivingRoom || currentArea == HouseArea.DownstairsHall)
            {
                // Ya casi peudo sentir el olor a queso caliente ...
                UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                    eventsObservationTextSO[4]
                );
            }
            else
            {
                // No debería faltar mucho para que la pizza esté lista ...
                UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                    eventsObservationTextSO[5]
                );
            }

            if (playerSoundsHandler == null) return;

            StartCoroutine(playerSoundsHandler.PlayPlayerSound(PlayerSoundsType.StomachGrowl));
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