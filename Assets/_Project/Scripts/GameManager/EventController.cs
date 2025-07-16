namespace TwelveG.GameManager
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using TwelveG.UIManagement;

    public class EventController : MonoBehaviour
    {
        [Header("Testing Settings")]
        [SerializeField] private GameObject afternoonEventsPrefab;
        [SerializeField] private GameObject eveningEventsPrefab;
        [SerializeField] private GameObject nightEventsPrefab;

        [Header("Testing Settings")]
        public bool freeRoam = false;
        public bool loadSpecificEvent = false;
        public int eventIndexToLoad = 0;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onImageCanvasControls;

        [Header("Text event SO")]

        private GameObject eventsParent = null;
        private List<GameEventBase> correspondingEvents = new List<GameEventBase>();
        private Transform playerContainerTransform;

        private int currentSceneIndex = 0;
        private int currentEventIndex = 0;

        private void Awake()
        {
            playerContainerTransform = GameObject.FindGameObjectWithTag("FreeRoam").GetComponent<Transform>();
        }

        void Start()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            InstantiateSceneEventsParent();
            PopulateEventsLists();
            VerifyRunTimeMode();
        }

        private void InstantiateSceneEventsParent()
        {
            switch (currentSceneIndex)
            {
                case 0: // Afternoon
                    eventsParent = Instantiate(afternoonEventsPrefab, this.transform);
                    break;
                case 1: // Evening
                    eventsParent = Instantiate(eveningEventsPrefab, this.transform);
                    break;
                case 2: // Night
                    eventsParent = Instantiate(nightEventsPrefab, this.transform);
                    break;
            }
        }

        private void PopulateEventsLists()
        {
            GameEventBase[] eventArray = eventsParent.GetComponentsInChildren<GameEventBase>();
            correspondingEvents = new List<GameEventBase>(eventArray);
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