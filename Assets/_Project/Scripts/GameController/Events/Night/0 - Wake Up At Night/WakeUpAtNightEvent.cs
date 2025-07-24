namespace TwelveG.GameController
{
    using System.Collections;
    using Cinemachine;
    using TwelveG.PlayerController;
    using TwelveG.UIController;
    using UnityEngine;

    public class WakeUpAtNightEvent : GameEventBase
    {
        //TODO: LOCALIZATION!
        [Header("Event options")]
        [TextArea(2, 25)]
        [SerializeField] private string defaultEventControlOptions;

        [Header("EventsSO references")]
        public GameEventSO onRainStart;
        public GameEventSO onControlCanvasControls;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onDialogCanvasShowDialog;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;


        private Animation animationComponent;

        public override IEnumerator Execute()
        {
            print("<------ WAKE UP AT NIGHT EVENT NOW -------->");
            onRainStart.Raise(this, null);

            // Parpadeo del jugador y activación de controles de menú
            // playerCapsule.SetActive(false);
            // playerVC.enabled = false;
            // bedVC.enabled = true;
            // playerShortcuts.enabled = false;

            // controlCanvasHandler.DeactivateControlCanvas();
            onControlCanvasControls.Raise(this, new ActivateCanvas(false));

            // mainCamera.SetActive(true);
            // cameraZoom.enabled = false;

            // yield return imageCanvasHandler.WakeUpBlinking();
            onImageCanvasControls.Raise(this, new WakeUpBlinking());
            yield return new WaitForSeconds(5f);

            // playerShortcuts.enabled = true;

            // Comentario sobre qué carajo hace en la cama
            // yield return dialogCanvasHandler.ShowDialog(eventIndex);
            onDialogCanvasShowDialog.Raise(this, "PRIMER DIALOGO DE WAKEUPNOCHE");
            yield return new WaitForSeconds(2f);

            // yield return dialogCanvasHandler.ShowDialog(eventIndex);
            onDialogCanvasShowDialog.Raise(this, "SEGUNDO DIALOGO DE WAKEUPNOCHE");
            yield return new WaitForSeconds(2f);

            // interactionCanvasHandler.ShowInteractionText("LEVANTARSE [E]");
            onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            // interactionCanvasHandler.HideText();
            onInteractionCanvasControls.Raise(this, new HideText());

            // animationComponent = bedVC.GetComponent<Animation>();
            animationComponent.enabled = true;
            animationComponent.Play();
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            // yield return imageCanvasHandler.FadeOutImage(0.5f);
            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);            

            // bedVC.enabled = false;
            // cameraZoom.enabled = true;

        }
    }

}