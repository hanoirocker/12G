using System.Collections;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.UIController;
using System.Collections.Generic;
using TwelveG.PlayerController;
using TwelveG.Utils;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;

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

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {

            // Esto es para actualizar los fallback texts para las Main Doors en particular
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[0]);
            // Actualizar text de ayuda del canvas del menu de pausa al presionar ESC
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

            GameObject zoomBird = PlayerHouseHandler.Instance.GetStoredObjectByID("ZoomBird");
            zoomBird.SetActive(true);
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            GameEvents.Common.onMainCameraSettings.Raise(this, new ResetCinemachineBrain());
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));

            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 2f));

            // Parece que algo pasó arriba, mejor reviso qué fue eso.
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[0]
            );

            yield return new WaitUntil(() => PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.Zoom);

            // Uff, voy a tener que limpiar esto si no quiero que vuelvan 
            // y me culpen por esta desastre.
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[1]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventsObservationTextSO[1].observationTextsStructure[0].observationText
            ));
            
            // Ahora hacemos interactuable el ave para que el jugador pueda limpiarla
            zoomBird.GetComponentInChildren<CleanBirdsHandler>().gameObject.GetComponent<Collider>().enabled = true;

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[1]);

            PlayerHouseHandler.Instance.GetStoredObjectByID("Broom").GetComponent<PickableItem>().canBePicked = true;
            PlayerHouseHandler.Instance.GetStoredObjectByID("Pickable - Empty Trash Bag (1)").GetComponent<PickableItem>().canBePicked = true;

            yield return new WaitUntil(() => PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.MiddleStairs);
            EnvironmentHandler.Instance.ToggleStoredPrefabs(new ObjectData("Crashing Bird", true));

            // Y ahora qué m...
            yield return new WaitForSeconds(1f);
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[2]
            );

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(1.5f);
            // Mejor tiro esta basura antes que ...
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[3]
            );

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[2]);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitForSeconds(1f);

            // A otra cosa.
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventsObservationTextSO[4]
            );

            // Reset manual de textos de Player Data Helper y mainDoorsFallacks a default
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
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
