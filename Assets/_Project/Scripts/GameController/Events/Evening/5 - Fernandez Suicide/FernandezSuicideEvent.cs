namespace TwelveG.GameController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Localization;
    using TwelveG.UIController;
  using TwelveG.PlayerController;

  public class FernandezSuicideEvent : GameEventBase
    {
        [Header("References")]
        [SerializeField] private GameObject suicideTriggerColliders;

        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onCinematicCanvasControls;
        [SerializeField] private GameEventSO onPlayerControls;
        [SerializeField] private GameEventSO onControlCanvasControls;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ F. SUICIDE EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);

            // Aca se instancian los coliders sobre ventanas y puertas
            // que den visualmente a la camioneta del vecino de enfrente.
            // Si el jugador los choca, se dispara evento carAlarmTrigger
            // recibido por Front House Pickup (Alarms) para el suicido.
            Instantiate(suicideTriggerColliders);

            // Unity Event (EventTriggeredByColliders - fernandezSuicide):
            // Se recibe cuando el jugador choca los colliders que provocan el suicidio
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));

            yield return new WaitForSeconds(1f);

            // Un disparo?
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            // Unity Event (CinematicBarsHandler - onCinematicBarsAnimationFinished):
            // Se recibe cuando las barras cinemáticas se terminan de mostrar
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onPlayerControls.Raise(this, new TogglePlayerCapsule(false));
            onPlayerControls.Raise(this, new TogglePlayerShortcuts(false));
            onControlCanvasControls.Raise(this, new EnableCanvas(false));
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