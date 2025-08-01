namespace TwelveG.GameController
{
    using System.Collections;
    using UnityEngine;
    using TwelveG.Localization;
    using TwelveG.UIController;
    using System.Collections.Generic;
    using TwelveG.PlayerController;
    using TwelveG.Utils;
    using Cinemachine;

    public class BirdsEvent : GameEventBase
    {
        [Header("Zoom references: ")]
        [SerializeField] private GameObject crashingBirdPrefab;

        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onObservationCanvasShowText;
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onPlayerControls;
        public GameEventSO onMainCameraSettings;

        [Header("Other eventsSO references")]
        public GameEventSO trashBagCanBePicked;
        public GameEventSO broomCanBePicked;
        public GameEventSO updateFallbackTexts;
        public GameEventSO destroyWindowToReplace;
        public GameEventSO zoomBirdIsInteractable;

        private bool allowNextAction = false;
        private int eventObservationTextIndex = 0;

        public override IEnumerator Execute()
        {
            print("<------ BIRDS EVENT NOW -------->");

            // Esto es para actualizar los fallback texts para las Main Doors en particular
            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            // Llega al Window to replace que deshabilita el mesh renderer de la ventana, y luego ejecuta
            // el InstantiateZoomBird del WindowToReplaceHandler.
            // zoomBird inicia con su collider interactuable apagado, se prende con el próximo eventoSO.
            destroyWindowToReplace.Raise(this, null);

            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, false));
            onPlayerControls.Raise(this, new TogglePlayerCapsule(true));

            onControlCanvasControls.Raise(this, new ActivateCanvas(true));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            yield return new WaitForSeconds(1f);

            // Parece que algo pasó arriba, mejor reviso qué fue eso.
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
            );
            eventObservationTextIndex += 1;

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Uff, voy a tener que limpiar esto si no quiero que vuelvan 
            // y me culpen por esta desastre.
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
            );
            eventObservationTextIndex += 1;
            yield return new WaitUntil(() => !DialogCanvasHandler.canvasIsShowing);

            // Se habilita el collider del interactuable de zoomBird
            zoomBirdIsInteractable.Raise(this, null);

            broomCanBePicked.Raise(this, true);

            trashBagCanBePicked.Raise(this, true);

            GameObject crashingBird = Instantiate(crashingBirdPrefab);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Y ahora qué m...
            yield return new WaitForSeconds(0.5f);
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
            );
            eventObservationTextIndex += 1;

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(1.5f);
            // Mejor tiro esta basura antes que ...
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
            );
            eventObservationTextIndex += 1;

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(1f);
            // A otra cosa.
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
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
