using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.UIController;
using TwelveG.PlayerController;
using TwelveG.EnvironmentController;
using TwelveG.Utils;

namespace TwelveG.GameController
{
    public class FernandezSuicideEvent : GameEventBase
    {
        [Header("References")]
        [SerializeField] private GameObject suicideTriggerColliders;

        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;
        public Transform suicideViewTransform;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ F. SUICIDE EVENT NOW -------->");

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            // YA FUE, me voy a lo de Mica.
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[0].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            // Aca se instancian los coliders sobre ventanas y puertas
            // que den visualmente a la camioneta del vecino de enfrente.
            // Si el jugador los choca, se dispara evento carAlarmTrigger
            // recibido por Front House Pickup (Alarms) para el suicido.
            Instantiate(suicideTriggerColliders);

            // Unity Event (EventTriggeredByColliders - fernandezSuicide):
            // Se recibe cuando el jugador choca los colliders que provocan el suicidio
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));

            yield return new WaitForSeconds(1f);

            // Ese sonido ...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[1]
            );

            // Unity Event (CinematicBarsHandler - onCinematicBarsAnimationFinished):
            // Se recibe cuando las barras cinemáticas se terminan de mostrar
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            GameEvents.Common.onPlayerDirectorControls.Raise(this, new ToggleTimelineDirector(1, true));

            yield return new WaitForSeconds(13f);
            // NO NO NO NO. MIERDA!
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[2]
            );

            // ACA SE ACTUALIZA LA POSICION DEL JUGADOR A LA VENTANA.
            Transform playerCapsuleTransform = GameObject.FindGameObjectWithTag("PlayerCapsule")
                .GetComponent<Transform>();

            playerCapsuleTransform.position = suicideViewTransform.position;
            playerCapsuleTransform.rotation = suicideViewTransform.rotation;

            yield return new WaitForSeconds(5.5f);
            // Esto no puede ser real
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[3]
            );

            // Unity Event (CinematicsHandler - onCutSceneFinished):
            // Se recibe cuando termina el cut scene
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            // TODO: introducir diálogo interno luego de observar el suicidio
            yield return new WaitForSeconds(1f);

            // No puede ser ...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[4]
            );

            GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(false));

            // Unity Event (CinematicBarsHandler - onCinematicBarsAnimationFinished):
            // Se recibe cuando las barras cinemáticas terminan de ocultarse
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));
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