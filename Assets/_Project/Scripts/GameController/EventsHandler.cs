using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TwelveG.UIController;
using TwelveG.AudioController;
using TwelveG.InteractableObjects;
using TwelveG.VFXController;
using System;

namespace TwelveG.GameController
{
    public class EventsHandler : MonoBehaviour
    {
        [Header("Events References")]
        [Space]
        [SerializeField] private GameObject introEvents;
        [SerializeField] private GameObject afternoonEvents;
        [SerializeField] private GameObject eveningEvents;
        [SerializeField] private GameObject nightEvents;

        [Header("Events Testing Settings")]
        [Space]
        [SerializeField] private bool freeRoam = false;
        [SerializeField] private bool loadSpecificEvent = false;
        [SerializeField] private EventsEnum eventEnumToLoad = EventsEnum.WakeUp;

        [Header("VFX Settings")]
        [Space]
        [Tooltip("Intensidad del efecto. 0 es desactivado, 1 es intensidad máxima.")]
        [SerializeField, Range(0f, 1f)] private float headacheVFXIntensity = 1f;

        [Header("Weather Testing Settings")]
        [Space]
        [SerializeField] private WeatherEvent weatherEvent = WeatherEvent.None;

        [Header("Items Testing Settings")]
        [Space]
        [SerializeField] private bool enableFlashlight = false;
        [SerializeField] private bool enableWalkieTalkie = false;

        private GameObject eventsParent = null;
        private List<GameEventBase> correspondingEvents = new List<GameEventBase>();
        private Transform playerCapsuleTransform;
        private int currentSceneIndex;
        private int currentEventIndex;
        private int eventIndexToLoad = 0;

        private Coroutine currentEventCoroutine;
        private GameEventBase currentExecutingEvent;
        private bool isEventRunning = false;

        // --------------------------------------------------------------------------------
        // 1. INICIALIZACIÓN Y CONFIGURACIÓN
        // --------------------------------------------------------------------------------

        public void BuildEvents()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            VerifySpecificTestSettings();
            InstantiateSceneEventsParent();
            PopulateEventsLists();
            VerifyRunTimeMode();
        }

        private void VerifySpecificTestSettings()
        {
            if (weatherEvent == WeatherEvent.SoftRain)
            {
                GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.SoftRain);
            }
            else if (weatherEvent == WeatherEvent.HardRain)
            {
                Debug.Log($"[EventsHandler]: isHardRaining checked but hard rain not worked yet!");
            }
            else if (weatherEvent == WeatherEvent.SoftWind)
            {
                GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.SoftWind);
            }
            else if (weatherEvent == WeatherEvent.HardWind)
            {
                GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.HardWind);
            }
        }

        private void InstantiateSceneEventsParent()
        {
            switch (currentSceneIndex)
            {
                case 0: // Intro
                    eventsParent = Instantiate(introEvents, this.transform);
                    break;
                case 2: // Afternoon
                    eventsParent = Instantiate(afternoonEvents, this.transform);
                    break;
                case 3: // Evening
                    eventsParent = Instantiate(eveningEvents, this.transform);
                    break;
                case 4: // Night
                    eventsParent = Instantiate(nightEvents, this.transform);
                    break;
                default:
                    Debug.LogError("[InstantiateSceneEventsParent]: Index not found");
                    break;
            }
        }

        private void PopulateEventsLists()
        {
            if (eventsParent != null) // Puede ser null cuando la escena es el Main Menu
            {
                GameEventBase[] eventArray = eventsParent.GetComponentsInChildren<GameEventBase>();
                correspondingEvents = new List<GameEventBase>(eventArray);
            }
        }

        // --------------------------------------------------------------------------------
        // 2. LÓGICA DE MODOS DE JUEGO (RUNTIME VS FREE ROAM)
        // --------------------------------------------------------------------------------

        private void VerifyRunTimeMode()
        {
            if (loadSpecificEvent)
            {
                StartCoroutine(SetStartingIndex(true));
                return;
            }
            else if (freeRoam)
            {
                ExecuteFreeRoam();
            }
            else
            {
                StartCoroutine(SetStartingIndex(false));
            }
        }

        private void ExecuteFreeRoam()
        {
            if (enableFlashlight)
                GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.Flashlight);
            if (enableWalkieTalkie)
                GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);

            Transform freeRoamTransform = GameObject.FindGameObjectWithTag("FreeRoam")
                .GetComponent<Transform>();
            playerCapsuleTransform = GameObject.FindGameObjectWithTag("PlayerCapsule")
                .GetComponent<Transform>();

            // Enviar Game Event SO sobre nuevo evento iniciado (Recibe por ejemplo Walkie Talkie para actualizar su estado)
            GameEvents.Common.onNewEventBegun.Raise(this, "FreeRoam");

            if (freeRoamTransform == null)
            {
                Debug.LogError("[EventsHandler]: FreeRoam prefab not found on scene!");
            }

            playerCapsuleTransform.position = freeRoamTransform.position;
            playerCapsuleTransform.rotation = freeRoamTransform.rotation;

            VFXManager.Instance?.SetResonanceIntensityMultiplier(headacheVFXIntensity);
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
        }

        private IEnumerator SetStartingIndex(bool isSpecificIndex)
        {
            if (isSpecificIndex)
            {
                ConvertEventEnumToIndex();

                // Reset de VFX al iniciar evento específico. Sus valores se actualizan desde los
                // eventos en sí.
                VFXManager.Instance?.SetResonanceIntensityMultiplier(0f);

                if (eventIndexToLoad > 1)
                {
                    GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
                }
            }

            yield return StartCoroutine(ExecuteEvents());
        }

        private void ConvertEventEnumToIndex()
        {
            for (int i = 0; i < correspondingEvents.Count; i++)
            {
                if (correspondingEvents[i].eventEnum == eventEnumToLoad)
                {
                    Debug.Log($"[EventsHandler]: Loading specific event: {eventEnumToLoad}, index {i}");
                    eventIndexToLoad = i;
                    break;
                }
            }
            currentEventIndex = eventIndexToLoad;
        }

        // --------------------------------------------------------------------------------
        // 3. CORE LOOP DE EVENTOS
        // --------------------------------------------------------------------------------

        private IEnumerator ExecuteEvents()
        {
            while (currentEventIndex < correspondingEvents.Count)
            {
                isEventRunning = true;
                currentExecutingEvent = correspondingEvents[currentEventIndex];
                currentExecutingEvent.gameObject.SetActive(true);
                SetUpCurrentEvent();

                VFXManager.Instance?.InitializeVFXSettings(currentExecutingEvent.eventEnum);
                // Ejecutar y almacenar referencia a la corrutina
                currentEventCoroutine = StartCoroutine(ExecuteSingleEvent(currentExecutingEvent));

                // Enviar Game Event SO sobre nuevo evento iniciado (Recibe por ejemplo Walkie Talkie para actualizar su estado)
                EventContextData eventContext = new EventContextData(
                    SceneUtils.RetrieveCurrentSceneEnum(),
                    currentExecutingEvent.eventEnum);
                GameEvents.Common.onNewEventBegun.Raise(this, eventContext);

                yield return currentEventCoroutine;

                currentEventCoroutine = null;
                currentExecutingEvent = null;
                isEventRunning = false;

                currentEventIndex++;
            }

            // Finalización normal de todos los eventos
            if (!loadSpecificEvent)
            {
                currentEventIndex = 0;
                GetComponent<SceneLoaderHandler>().LoadNextSceneSequence(currentSceneIndex + 1);
            }
        }

        private void SetUpCurrentEvent()
        {
            GameEventListener[] eventListeners = correspondingEvents[currentEventIndex].GetComponents<GameEventListener>();
            List<GameEventListener> listeners = new List<GameEventListener>(eventListeners);
            foreach (GameEventListener listener in listeners)
            {
                listener.enabled = true;
            }
        }

        private IEnumerator ExecuteSingleEvent(GameEventBase gameEvent)
        {
            yield return StartCoroutine(gameEvent.Execute());
            gameEvent.gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------------------
        // 4. CONTROL DE SALTO DE EVENTOS
        // --------------------------------------------------------------------------------

        public void SkipCurrentEvent(Component sender, object data)
        {
            Debug.Log($"[EventsHandler]: Recibido evento por {sender.name} para saltar al siguiente evento.");
            SkipToNextEvent();
        }

        private void SkipToNextEvent()
        {
            if (!isEventRunning || currentEventCoroutine == null)
            {
                Debug.LogWarning("[EventsHandler]: No hay evento en ejecución para saltar.");
                return;
            }

            Debug.Log($"[EventsHandler]: Saltando evento actual: {currentExecutingEvent?.name}");

            StopCoroutine(currentEventCoroutine);

            if (currentExecutingEvent != null)
            {
                Destroy(currentExecutingEvent.gameObject);
            }

            currentEventIndex++;
            isEventRunning = false;

            // Iniciar el siguiente evento
            if (currentEventIndex < correspondingEvents.Count)
            {
                StartCoroutine(ExecuteEvents());
            }
            else
            {
                Debug.Log("[EventsHandler]: No hay más eventos después de saltar.");
                // Si estamos en modo test event index, no cargar próxima escena
                if (!loadSpecificEvent)
                {
                    currentEventIndex = 0;
                    GetComponent<SceneLoaderHandler>().LoadNextSceneSequence(currentSceneIndex + 1);
                }
            }
        }

        // --------------------------------------------------------------------------------
        // 5. GETTERS Y UTILIDADES
        // --------------------------------------------------------------------------------

        public int RetrieveCurrentEventIndex()
        {
            return currentEventIndex;
        }
    }
}