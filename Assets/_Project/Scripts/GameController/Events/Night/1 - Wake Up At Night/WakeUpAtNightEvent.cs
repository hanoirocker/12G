namespace TwelveG.GameController
{
    using System.Collections;
    using TwelveG.AudioController;
    using TwelveG.UIController;
    using UnityEngine;

    public class WakeUpAtNightEvent : GameEventBase
    {
        [Header("EventsSO references")]
        public GameEventSO onStartWeatherEvent;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onDialogCanvasShowDialog;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;

        // private bool allowNextAction = false;
        private Animation animationComponent;

        public override IEnumerator Execute()
        {
            print("<------ WAKE UP AT NIGHT EVENT NOW -------->");
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.HardRain);

            // Parpadeo del jugador y activación de controles de menú
            // playerCapsule.SetActive(false);
            // playerVC.enabled = false;
            // bedVC.enabled = true;
            // playerShortcuts.enabled = false;

            // controlCanvasHandler.DeactivateControlCanvas();

            // mainCamera.SetActive(true);
            // cameraZoom.enabled = false;

            // yield return imageCanvasHandler.WakeUpBlinking();
            GameEvents.Common.onImageCanvasControls.Raise(this, new WakeUpBlinking());
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
            GameEvents.Common.onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            // interactionCanvasHandler.HideText();
            GameEvents.Common.onInteractionCanvasControls.Raise(this, new HideText());

            // animationComponent = bedVC.GetComponent<Animation>();
            animationComponent.enabled = true;
            animationComponent.Play();
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            // yield return imageCanvasHandler.FadeOutImage(0.5f);
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);

            // bedVC.enabled = false;
            // cameraZoom.enabled = true;
        }

        // public void AllowNextActions(Component sender, object data)
        // {
        //     print(gameObject.name + " recieved event sent by: " + sender.gameObject.name);
        //     allowNextAction = true;
        // }

        // public void ResetAllowNextActions()
        // {
        //     allowNextAction = false;
        // }
    }

}