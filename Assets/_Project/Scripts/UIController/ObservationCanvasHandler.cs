namespace TwelveG.UIController
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

    public class ObservationCanvasHandler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Tiempo máximo que un texto de interacción puede esperar mientras se muestra un texto de evento.")]
        public float overlapAllowedTime = 2f;

        private Canvas observationCanvas;
        private TextMeshProUGUI observationCanvasText;
        private ObservationTextSO lastObservationTextSORecieved = null;

        // Cola de pedidos
        private Queue<ObservationRequest> requestQueue = new Queue<ObservationRequest>();

        // Coroutine que procesa la cola
        private Coroutine processorCoroutine;

        // Wrapper simple para almacenar encolado y tiempo
        private class ObservationRequest
        {
            public ObservationTextSO so;
            public float enqueueTime;
            public ObservationRequest(ObservationTextSO so, float enqueueTime)
            {
                this.so = so;
                this.enqueueTime = enqueueTime;
            }
        }

        private void Awake()
        {
            observationCanvas = GetComponent<Canvas>();
            observationCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
        }

        void Start()
        {
            observationCanvas.enabled = false;
        }

        public bool IsShowingText()
        {
            return observationCanvas.isActiveAndEnabled;
        }

        // Entrada pública: encolar el texto
        public void ShowObservationText(Component sender, object data)
        {
            // Debug.Log("[ObservationCanvasHandler] ShowObservationText called by " + sender.name);
            if (data == null) return;

            var so = data as ObservationTextSO;
            if (so == null) return;

            // Si llega un texto de evento, preempte (si hay un texto de interacción en pantalla)
            if (so.isEventText)
            {
                // Si actualmente se está mostrando un texto de interacción, cortarlo y limpiar UI
                if (lastObservationTextSORecieved != null && !lastObservationTextSORecieved.isEventText)
                {
                    // Detener procesador (si está corriendo) y limpiar UI para evitar bloqueos
                    if (processorCoroutine != null)
                    {
                        StopCoroutine(processorCoroutine);
                        processorCoroutine = null;
                    }

                    observationCanvas.enabled = false;
                    observationCanvasText.text = "";
                    lastObservationTextSORecieved = null;

                    // Vaciar cola
                    requestQueue.Clear();
                }

                // Para eventos, preferimos mostrarlo lo antes posible: lo ponemos al frente vaciando la cola
                requestQueue.Enqueue(new ObservationRequest(so, Time.time));
            }
            else
            {
                // Texto de interacción
                requestQueue.Enqueue(new ObservationRequest(so, Time.time));
            }

            // Si no hay procesador en marcha, arrancarlo
            if (processorCoroutine == null)
            {
                processorCoroutine = StartCoroutine(ProcessQueueCoroutine());
            }
        }

        private IEnumerator ProcessQueueCoroutine()
        {
            while (requestQueue.Count > 0)
            {
                var req = requestQueue.Dequeue();

                // Si el request es de interacción y ha esperado demasiado desde que se encoló, lo descartamos
                if (!req.so.isEventText)
                {
                    float waitSinceEnqueue = Time.time - req.enqueueTime;
                    if (waitSinceEnqueue > overlapAllowedTime)
                    {
                        // Saltamos este request
                        continue;
                    }

                    // Si actualmente está mostrando un evento, esperar su final hasta overlapAllowedTime
                    if (lastObservationTextSORecieved != null && lastObservationTextSORecieved.isEventText)
                    {
                        float t0 = Time.time;
                        // Esperamos que el canvas deje de estar activo o que se supere el tiempo máximo
                        yield return new WaitUntil(() => !observationCanvas.enabled || (Time.time - t0) > overlapAllowedTime);

                        // Si superó el tiempo, descartamos
                        if ((Time.time - t0) > overlapAllowedTime)
                        {
                            continue;
                        }
                        // Si el canvas se desactivó, seguimos normal
                    }
                }
                else
                {
                    // Si es un texto de evento, garantizar que no quede nada de interacción en pantalla
                    if (lastObservationTextSORecieved != null && !lastObservationTextSORecieved.isEventText)
                    {
                        // limpiamos para mostrar el evento inmediatamente
                        observationCanvas.enabled = false;
                        observationCanvasText.text = "";
                        lastObservationTextSORecieved = null;
                    }
                }

                // Mostrar texto
                lastObservationTextSORecieved = req.so;

                string textToShow = Utils.TextFunctions.RetrieveObservationText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    lastObservationTextSORecieved
                );

                observationCanvasText.text = textToShow;
                observationCanvas.enabled = true;

                // Mostrar durante el tiempo calculado (interruptible por nueva preempción)
                float displayDuration = Utils.TextFunctions.CalculateTextDisplayDuration(textToShow);
                float elapsed = 0f;

                while (elapsed < displayDuration)
                {
                    // Si llega un nuevo evento que debe preemptar (la ShowObservationText lo hará),
                    // se puede detener este procesador desde ShowObservationText (que hace StopCoroutine).
                    // Chequeamos que el canvas siga siendo el nuestro y que lastObservationTextSORecieved siga siendo el mismo.
                    if (lastObservationTextSORecieved == null || observationCanvasText.text != textToShow)
                    {
                        // Fue preemptado o limpiado externamente
                        break;
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // Cleanup y cerramos canvas si se seguía mostrando
                if (observationCanvas.enabled && observationCanvasText.text == textToShow)
                {
                    observationCanvas.enabled = false;
                    observationCanvasText.text = "";
                }

                // Evitar flicker esperando un pequeño tiempo
                yield return new WaitForSeconds(0.05f);
            }

            // Marcamos procesador null al tener la cola vacía
            processorCoroutine = null;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (lastObservationTextSORecieved == null) return;

            string textToShow = Utils.TextFunctions.RetrieveObservationText(
                LocalizationManager.Instance.GetCurrentLanguageCode(),
                lastObservationTextSORecieved
            );
            observationCanvasText.text = textToShow;
        }
    }
}
