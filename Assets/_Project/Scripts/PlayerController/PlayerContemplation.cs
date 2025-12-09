namespace TwelveG.PlayerController
{
    using System;
    using System.Collections;
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerContemplation : MonoBehaviour
    {
        [Header("Raycast settings")]
        [SerializeField] private LayerMask contemplableMask;
        [SerializeField, Range(0.5f, 5f)] private float contemplationRange = 3f;
        public Color raycastColor;

        [Header("Cameras Settings")]
        [SerializeField] private Transform interactorSource;

        [Header("EventsSO references")]
        public GameEventSO onContemplationCanvasControls;
        public GameEventSO onContemplationCanvasShowText;

        private CameraZoom cameraZoom;
        private IContemplable lastContemplatedObject = null;
        private int defaultTextCounter = 0;

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
                onContemplationCanvasControls.Raise(this, new EnableCanvas(false));
                lastContemplatedObject?.IsAbleToBeContemplate(true);
            }
        }

        private void ContemplateObject()
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);

            if (Physics.Raycast(r, out RaycastHit hitInfo, contemplationRange, contemplableMask))
            {
                bool hasContemplable = hitInfo.collider.gameObject.TryGetComponent(out IContemplable contemplableObj);

                if (hasContemplable && contemplableObj.CanBeInteractedWith())
                {
                    // Si estoy contemplando un objeto distinto al anterior
                    if (lastContemplatedObject != null && lastContemplatedObject != contemplableObj)
                    {
                        lastContemplatedObject.IsAbleToBeContemplate(true);
                    }

                    lastContemplatedObject = contemplableObj;
                    StartCoroutine(ContemplationCoroutine(contemplableObj));
                }
            }
            else
            {
                // Si no golpeamos nada, re-habilitamos el último contemplado
                if (lastContemplatedObject != null)
                {
                    lastContemplatedObject.IsAbleToBeContemplate(true);
                    lastContemplatedObject = null;
                }

                onContemplationCanvasControls.Raise(this, new EnableCanvas(false));
            }
        }

        private void OnDisable()
        {
            onContemplationCanvasControls.Raise(this, new EnableCanvas(false));
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
                contemplationText = contemplableObj.GetContemplationText();

                onContemplationCanvasShowText.Raise(this, contemplationText);
            }
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
