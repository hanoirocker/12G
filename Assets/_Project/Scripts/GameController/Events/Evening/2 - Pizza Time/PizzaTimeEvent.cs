namespace TwelveG.GameController
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.UIController;
    using TwelveG.Utils;
    using UnityEngine;

    public class PizzaTimeEvent : GameEventBase
    {
        [Header("Event options")]
        [TextArea(5, 50)]
        [SerializeField, Range(1, 10)] private int initialTime = 4;

        [Header("Objects options")]
        [SerializeField] private GameObject policeCar;

        [Header("Audio settings")]
        [SerializeField] private AudioClip chairMovingSound = null;

        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;
        [SerializeField] private EventsControlCanvasInteractionTextSO eventsControlCanvasInteractionTextSO_eating;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onImageCanvasControls;
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onEventInteractionCanvasShowText;
        [SerializeField] private GameEventSO onInteractionCanvasShowText;
        [SerializeField] private GameEventSO onInteractionCanvasControls;
        [SerializeField] private GameEventSO onPlayerControls;
        [SerializeField] private GameEventSO onControlCanvasControls;
        [SerializeField] private GameEventSO onControlCanvasSetInteractionOptions;
        [SerializeField] private GameEventSO onVirtualCamerasControl;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO plateCanBePicked;
        [SerializeField] private GameEventSO pizzaCanBePicked;
        [SerializeField] private GameEventSO updateFallbackTexts;

        private AudioSource audioSource;
        private bool allowNextAction = false;
        private int eventObservationTextIndex = 0;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override IEnumerator Execute()
        {
            print("<------ PIZZA TIME EVENT NOW -------->");

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            yield return new WaitForSeconds(initialTime);
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
            );
            eventObservationTextIndex += 1;

            // Esto notifica al plato para que haga AllowItemToBePicked = true;
            pizzaCanBePicked.Raise(this, true);
            plateCanBePicked.Raise(this, true);

            // Unity Event (MicrowaveHandler - pizzaHeatingFinished):
            // Se muestra el diálogo de felicidad del pelotudo
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(0.5f);
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO[eventObservationTextIndex]
            );

            // Unity Event (SitAndEat - sitAndEatPizza):
            // Al interactuar con SitAndEat (mesa de la cocina) se deshabilitan controles pples 
            // del jugador y se pasa a la VC KitchenDeskVC
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));


            if (chairMovingSound)
            {
                audioSource.PlayOneShot(chairMovingSound);
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
            else
            {
                // Espera lo que tarda por defecto el FadeOutImage
                yield return new WaitForSeconds(1);
            }

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDesk, true));

            onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            yield return new WaitForSeconds(1f);

            onControlCanvasSetInteractionOptions.Raise(
                this,
                eventsControlCanvasInteractionTextSO_eating
            );

            onControlCanvasControls.Raise(this, new ActivateCanvas(true));
            onControlCanvasControls.Raise(this, new EnableCanvas(true));

            onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            // Unity Event (PizzaSliceHandler - instantiatePoliceCar)
            // Avisa que va por la segunda mordida y se debe instanciar el auto de policia
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            Instantiate(policeCar);

            // Unity Event (PizzaSliceHandler - finishedEatingPizza):
            // Se invoca finishedEatingPizza al final de comer la pizza para que el jugador vuelva
            // a estar disponible.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onControlCanvasControls.Raise(this, new ResetControlCanvasSpecificOptions());
            yield return new WaitForSeconds(3.5f);
            onControlCanvasControls.Raise(this, new EnableCanvas(false));
            // LEVANTARSE [E]
            onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );
            // Espera hasta que el jugador presione nuevamente la E
            // para levantarse de la silla.
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            onInteractionCanvasControls.Raise(this, new HideText());

            onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));
            onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 2f));

            yield return new WaitForSeconds(2f);

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDesk, false));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 2f));

            yield return new WaitForSeconds(1f);

            onPlayerControls.Raise(this, new EnablePlayerControllers(true));
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