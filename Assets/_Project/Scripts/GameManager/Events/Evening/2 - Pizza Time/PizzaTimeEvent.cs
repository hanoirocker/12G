namespace TwelveG.GameManager
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
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
        [SerializeField] private List<ObservationTextSO> eventsObservationTextSO;
        [SerializeField] private EventsControlCanvasInteractionTextSO eventsControlCanvasInteractionTextSO_eating;
        [SerializeField] private EventsControlCanvasInteractionTextSO eventsControlCanvasInteractionTextSO_standup;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onImageCanvasControls;
        [SerializeField] private GameEventSO onObservationCanvasShowText;
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

            onImageCanvasControls.Raise(this, "FadeOutImage");

            onPlayerControls.Raise(this, "DisablePlayerCapsule");

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

            onVirtualCamerasControl.Raise(this, "EnableKitchenDeskVC");

            onPlayerControls.Raise(this, "EnableHeadLookAround");

            onImageCanvasControls.Raise(this, "FadeInImage");
            yield return new WaitForSeconds(1f);

            onControlCanvasSetInteractionOptions.Raise(
                this,
                eventsControlCanvasInteractionTextSO_eating
            );

            onControlCanvasControls.Raise(this, "ActivateControlCanvas");
            onControlCanvasControls.Raise(this, "ShowControlCanvas");

            onPlayerControls.Raise(this, "EnablePlayerShortcuts");

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

            onControlCanvasSetInteractionOptions.Raise(
                this,
                eventsControlCanvasInteractionTextSO_standup
            );

            // Espera hasta que el jugador presione nuevamente la E
            // para levantarse de la silla.
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onInteractionCanvasControls.Raise(this, "HideText");

            onPlayerControls.Raise(this, "DisableHeadLookAround");

            onControlCanvasControls.Raise(this, "ResetControlCanvasSpecificOptions");

            onControlCanvasControls.Raise(this, "HideControlCanvas");

            onImageCanvasControls.Raise(this, "FadeOutImage2");
            yield return new WaitForSeconds(2f);

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

            onVirtualCamerasControl.Raise(this, "DisableKitchenDeskVC");

            onImageCanvasControls.Raise(this, "FadeInImage2");

            yield return new WaitForSeconds(2f);
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