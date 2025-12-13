using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.Utils;

namespace TwelveG.GameController
{
    public class WalkieTalkieQuestEvent : GameEventBase
    {
        [Header("Event references")]
        [SerializeField] private AudioClip oldRadioClip;

        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO[] mainDoorsFallbacksTextsSO;
        [SerializeField] private List<UIOptionsTextSO> playerHelperDataTextSO;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO drawerCanBeInteracted;
        [SerializeField] private GameEventSO triggerOldRadio;
        [SerializeField] private GameEventSO triggerHouseLightsFlickering;

        private bool allowNextAction = false;
        private bool bookHasBeenExamined = false;
        private bool safeBoxNoteHasBeenExamined = false;
        private bool parentsPortraitHasBeenExamined = false;

        public override IEnumerator Execute()
        {
            print("<------ WT QUEST EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[0]);

            // Tengo que encontrar la forma de hablar con Mica como sea.
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);
            yield return new WaitForSeconds(12f);
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            // TODO: reset mainDoorsFallback texts

            // Mi Walkie Talkie! Si no mal recuerdo mi madre lo había escondido ...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[1]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[1].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[1]);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

            // Las bromas que jugabamos a la policia ya no son graciosas ...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[2]
            );

            drawerCanBeInteracted.Raise(this, null);

            // Unity Event (SafeBoxHandler - safeBoxNoteCanBeExamine):
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[2]);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);

            // Los sucesos como la radio que se prende sola y la puerta
            // que se cierra del golpe, se disparan por medio de game event SO enviados
            // por el retrato de casamiento y por los colliders instanciados luego de examinar
            // el libro en el living.
            // Unity Event (PickableItem - walkieTalkiePickedUp) en Pickable - Walkie Talkie:
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // TODO: reset el Player Helper Data y mainDoorsFallback texts

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            // Se levanta viento mas fuerte
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.HardWind);

            // Parpadean luces de la casa nuevamente
            triggerHouseLightsFlickering.Raise(this, 10f);
        }

        public void OnSagaBookExamined(Component sender, object data)
        {
            if (!bookHasBeenExamined)
            {
                bookHasBeenExamined = true;
                // Aca sucede el flickering de las luces recibido y disparado por el PlayerHouseHandler
                triggerHouseLightsFlickering.Raise(this, 5f);
            }
        }

        public void OnSafeBoxNoteExamined(Component sender, object data)
        {
            if (!safeBoxNoteHasBeenExamined)
            {
                GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[3]);
                GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);
                safeBoxNoteHasBeenExamined = true;
            }
        }

        public void OnParentsPortraitExamined(Component sender, object data)
        {
            if (!parentsPortraitHasBeenExamined)
            {
                parentsPortraitHasBeenExamined = true;
                // Se enciende la Old Radio pasandole el clip a reproducir
                triggerOldRadio.Raise(this, oldRadioClip);
            }
        }

        public void AllowNextActions(Component sender, object data)
        {
            Debug.Log("[WalkieTalkieQuestEvent] Allowing next action by event from: " + sender.gameObject.name);
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }
    }
}