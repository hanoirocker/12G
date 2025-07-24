namespace TwelveG.GameController
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using TwelveG.UIController;

    public class EventController : MonoBehaviour
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

        [Header("EventsSO references")]
        public GameEventSO onImageCanvasControls;
        public GameEventSO onRainStart;

        [Header("Text event SO")]

        private GameObject eventsParent = null;
        private List<GameEventBase> correspondingEvents = new List<GameEventBase>();
        // private Transform playerContainerTransform;

        private int currentSceneIndex;
        private int currentEventIndex;

        // private void Awake()
        // {
        //     playerContainerTransform = GameObject.FindGameObjectWithTag("FreeRoam").GetComponent<Transform>();
        // }

        void Start()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex == 1) // Main Menu Scene
            {
                
                return;
            }

            VerifySpecificTestSettings();
            InstantiateSceneEventsParent();
            PopulateEventsLists();
            VerifyRunTimeMode();
        }

        private void VerifySpecificTestSettings()
        {
            if (isRaining)
            {
                onRainStart.Raise(this, null);
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

        public bool loadSpecificEventEnabled()
        {
            return loadSpecificEvent;
        }

        private void VerifyRunTimeMode()
        {
            if (loadSpecificEvent)
            {
                StartCoroutine(ExecuteSpecificEvent());
                return;
            }
            else if (freeRoam)
            {
                ExecuteFreeRoam();
            }
            else
            {
                StartCoroutine(ExecuteEvents());
            }
        }

        private void ExecuteFreeRoam()
        {
            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));

            // FOR TESTING;
        }

        private IEnumerator ExecuteEvents()
        {
            while (currentEventIndex < correspondingEvents.Count)
            {
                correspondingEvents[currentEventIndex].gameObject.SetActive(true);
                SetUpCurrentEvent();
                yield return StartCoroutine(correspondingEvents[currentEventIndex].Execute());
                Destroy(correspondingEvents[currentEventIndex].gameObject);
                currentEventIndex++;
            }
        }

        // La idea de esta función es que antes que se ejecute la corrutina de cada evento base
        // se carguen sus dependencias y se activen todos los GameEventListeners en el mismo,
        // que por defecto están apagados hasta que se indexe el evento.
        private void SetUpCurrentEvent()
        {
            GameEventListener[] eventListeners = correspondingEvents[currentEventIndex].GetComponents<GameEventListener>();
            List<GameEventListener> listeners = new List<GameEventListener>(eventListeners);
            foreach (GameEventListener listener in listeners)
            {
                listener.enabled = true;
            }
        }

        public IEnumerator ExecuteSpecificEvent()
        {
            currentEventIndex = eventIndexToLoad;

            if (eventIndexToLoad >= 0 && eventIndexToLoad < correspondingEvents.Count)
            {
                correspondingEvents[currentEventIndex].gameObject.SetActive(true);
                SetUpCurrentEvent();
                onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
                yield return StartCoroutine(correspondingEvents[eventIndexToLoad].Execute());
            }
        }
    }
}