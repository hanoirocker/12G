namespace TwelveG.GameManager
{
    using System.Collections;
    using UnityEngine;
    using TwelveG.Localization;
    using TwelveG.UIManagement;

    public class BirdsEvent : GameEventBase
    {
        //TODO: LOCALIZATION! eventtextsSO
        [Header("Zoom references: ")]
        [SerializeField] private GameObject zoomBirdPrefab;
        [SerializeField] private GameObject crashingBirdPrefab;

        [Header("Texts references: ")]
        [SerializeField] private EventsFallbacksTextsSO mainDoorsFallbacksTextsSO;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onObservationCanvasShowText;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onPlayerControls;

        [Header("Other Events references: ")]
        public GameEventSO trashBagCanBePicked;
        public GameEventSO broomCanBePicked;
        public GameEventSO updateFallbackTexts;
        public GameEventSO destroyWindowToReplace;
        public GameEventSO zoomBirdIsInteractable;

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

            onVirtualCamerasControl.Raise(this, "EnablePlayerVC");

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

            onControlCanvasControls.Raise(this, "ActivateControlCanvas");

            onImageCanvasControls.Raise(this, "FadeInImage");
            yield return new WaitForSeconds(1f);

            onObservationCanvasShowText.Raise(this, "PRIMER DIALOGO DEL COSO");

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onObservationCanvasShowText.Raise(this, "SEGUNDO DIALOGO DEL COSO");
            yield return new WaitUntil(() => !DialogCanvasHandler.canvasIsShowing);

            // Se habilita el collider del interactuable de zoomBird
            zoomBirdIsInteractable.Raise(this, null);

            broomCanBePicked.Raise(this, true);

            trashBagCanBePicked.Raise(this, true);

            GameObject crashingBird = Instantiate(crashingBirdPrefab);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(0.5f);
            onObservationCanvasShowText.Raise(this, "TERCER DIALOGO DEL COSO");

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(1.5f);
            onObservationCanvasShowText.Raise(this, "CUARTO DIALOGO DEL COSO");

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
            yield return new WaitForSeconds(1f);
            onObservationCanvasShowText.Raise(this, "QUINTO DIALOGO DEL COSO");
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
