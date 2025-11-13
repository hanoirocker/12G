using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TwelveG.UIController;
using TwelveG.AudioController;

namespace TwelveG.GameController
{
    public class EventsHandler : MonoBehaviour
    {
        [Header("Testing Settings")]
        public GameObject introEvents;
        public GameObject afternoonEvents;
        public GameObject eveningEvents;
        public GameObject nightEvents;

        [Header("Testing Settings")]
        public bool freeRoam = false;
        public bool loadSpecificEvent = false;
        public int eventIndexToLoad = 0;
        public bool isRaining = false;
        public bool isWindBlowing = false;

        [Header("EventsSO references")]
        public GameEventSO onImageCanvasControls;
        public GameEventSO onDeactivateCanvas;
        public GameEventSO StartWeatherEvent;
        public GameEventSO onNewEventBegun;

        [Header("Text event SO")]
        private GameObject eventsParent = null;
        private List<GameEventBase> correspondingEvents = new List<GameEventBase>();
        private Transform playerCapsuleTransform;
        private int currentSceneIndex;
        private int currentEventIndex;

        private Coroutine currentEventCoroutine;
        private GameEventBase currentExecutingEvent;
        private bool isEventRunning = false;

        private IEnumerator ExecuteEvents()
        {
            while (currentEventIndex < correspondingEvents.Count)
            {
                isEventRunning = true;
                currentExecutingEvent = correspondingEvents[currentEventIndex];
                currentExecutingEvent.gameObject.SetActive(true);
                SetUpCurrentEvent();

                // Ejecutar y almacenar referencia a la corrutina
                currentEventCoroutine = StartCoroutine(ExecuteSingleEvent(currentExecutingEvent));
                Debug.Log($"[EventsHandler]: Iniciando evento: {currentExecutingEvent?.name}");

                // Enviar Game Event SO sobre nuevo evento iniciado (Recibe por ejemplo Walkie Talkie para actualizar su estado)
                EventContextData eventContext = new EventContextData(
                    GameManager.Instance.RetrieveCurrentSceneEnum(),
                    currentExecutingEvent.eventEnum);
                onNewEventBegun.Raise(this, eventContext);

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

        private IEnumerator ExecuteSingleEvent(GameEventBase gameEvent)
        {
            yield return StartCoroutine(gameEvent.Execute());
            gameEvent.gameObject.SetActive(false);
        }

        private IEnumerator SetStartingIndex(bool isSpecificIndex)
        {
            if (isSpecificIndex)
            {
                currentEventIndex = eventIndexToLoad;
                if (eventIndexToLoad > 1)
                {
                    onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
                }
            }

            yield return StartCoroutine(ExecuteEvents());
        }

        private void VerifySpecificTestSettings()
        {
            if (isRaining)
            {
                StartWeatherEvent.Raise(this, WeatherEvent.SoftRain);
            }
            if (isWindBlowing)
            {
                StartWeatherEvent.Raise(this, WeatherEvent.SoftWind);
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
            Transform freeRoamTransform = GameObject.FindGameObjectWithTag("FreeRoam")
                .GetComponent<Transform>();
            playerCapsuleTransform = GameObject.FindGameObjectWithTag("PlayerCapsule")
                .GetComponent<Transform>();

            if (freeRoamTransform == null)
            {
                Debug.LogError("[EventController]: FreeRoam prefab not found on scene!");
            }

            playerCapsuleTransform.position = freeRoamTransform.position;
            playerCapsuleTransform.rotation = freeRoamTransform.rotation;
            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
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

        public void BuildEvents()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            VerifySpecificTestSettings();
            InstantiateSceneEventsParent();
            PopulateEventsLists();
            VerifyRunTimeMode();
        }

        public void SkipCurrentEvent(Component sender, object data)
        {
            Debug.Log($"[EventsHandler]: Recibido evento por {sender.name} para saltar al siguiente evento.");
            SkipToNextEvent();
        }

        public int RetrieveCurrentEventIndex()
        {
            return currentEventIndex;
        }
    }
}