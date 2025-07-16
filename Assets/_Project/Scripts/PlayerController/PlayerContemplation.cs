namespace TwelveG.PlayerController
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
  using TwelveG.UIManagement;
  using UnityEngine;

    public class PlayerContemplation : MonoBehaviour
    {
        [Header("Raycast settings")]
        [SerializeField] private LayerMask contemplableMask;
        [SerializeField, Range(0.5f, 5f)] private float contemplationRange = 1.2f;
        public Color raycastColor;

        [Header("Cameras Settings")]
        [SerializeField] private Transform interactorSource;

        // [Header("Contemplation Settings")]
        // [SerializeField, Range(0, 6)] int defaultContemplations = 3;
        // [SerializeField] List<String> defaultContemplationTexts = new List<String>();

        [Header("EventsSO references")]
        public GameEventSO onContemplationCanvasControls;
        public GameEventSO onContemplationCanvasShowText;

        private CameraZoom cameraZoom;
        private IContemplable lastContemplatedObject = null;
        private int defaultTextCounter = 0;
        private string canvasText;

        private void Awake()
        {
            cameraZoom = GetComponent<CameraZoom>();
        }

        private void Start()
        {   
            onContemplationCanvasControls.Raise(this, new EnableCanvas(false));
        }

        private void Update()
        {
            if (cameraZoom.playerIsZooming())
            {
                ContemplateObject();
            }
            else
            {
                // TODO (OPTIMIZE): HACER ALGO CON ESTO, no se puede enviar una señal por frame
                // si el jugador no está presionando el botón derecho!
                onContemplationCanvasControls.Raise(this, new EnableCanvas(false));
                lastContemplatedObject?.IsAbleToBeContemplate(true);
            }
        }

        private void ContemplateObject()
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, contemplationRange, contemplableMask))
            {
                bool objectHasContemplableComponent = hitInfo.collider.gameObject.TryGetComponent(out IContemplable contemplableObj);
                if (objectHasContemplableComponent && contemplableObj!.CanBeInteractedWith())
                {
                    lastContemplatedObject = contemplableObj;
                    StartCoroutine(ContemplationCoroutine(contemplableObj));
                }
            }
            else
            {
                return;
            }
        }

        private IEnumerator ContemplationCoroutine(IContemplable contemplableObj)
        {
            defaultTextCounter = contemplableObj.RetrieveDefaultTextCounter();
            contemplableObj.IsAbleToBeContemplate(false);
            string contemplationText;
            yield return new WaitForSeconds(0.25f);

            // Si el objeto puede ser contemplado varias veces, muestran los textos del objeto
            // Logica dentro de ContemplateObject.
            if (!contemplableObj.HasReachedMaxContemplations())
            {
                contemplationText = contemplableObj.GetContemplationText(LocalizationManager.Instance.GetCurrentLanguageCode());

                onContemplationCanvasShowText.Raise(this, contemplationText);
            }
            // Si ya no posee textos de contemplación, se muestran textos default contemplations.
            // else if (contemplableObj.HasReachedMaxContemplations() && defaultTextCounter < defaultContemplations)
            // {
            //     contemplableObj.UpdateDefaultTextCounter();
            //     int randomAnswer = UnityEngine.Random.Range(0, defaultContemplationTexts.Count);
            //     contemplationText = defaultContemplationTexts[randomAnswer];

            //     onContemplationCanvasShowText.Raise(this, contemplationText);
            // }
            else
            {
                contemplableObj.IsAbleToBeContemplate(false);
            }
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = raycastColor;
            Vector3 direction = interactorSource.TransformDirection(Vector3.forward) * contemplationRange;
            Gizmos.DrawRay(interactorSource.position, direction);
        }
    }
}
