using System;
using System.Collections;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.PlayerController
{
    public class PlayerContemplation : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 2f)] private float contemplationDelay = 0.5f;

        [Space(10)]
        [Header("Raycast settings")]
        [SerializeField] private LayerMask contemplableMask;
        [SerializeField, Range(0.5f, 5f)] private float contemplationRange = 3f;
        public Color raycastColor;

        [Space(10)]
        [Header("Cameras Settings")]
        [SerializeField] private Transform interactorSource;

        private CameraZoom cameraZoom;
        private IContemplable lastContemplatedObject = null;

        private void Awake()
        {
            cameraZoom = GetComponent<CameraZoom>();
        }

        private void Update()
        {
            if (cameraZoom.playerIsZooming())
            {
                ContemplateObject();
            }
            else
            {
                if (lastContemplatedObject != null)
                {
                    lastContemplatedObject.IsAbleToBeContemplate(true);
                    lastContemplatedObject = null;
                }
            }
        }

        private void ContemplateObject()
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);

            if (Physics.Raycast(r, out RaycastHit hitInfo, contemplationRange, contemplableMask))
            {
                bool hasContemplable = hitInfo.collider.gameObject.TryGetComponent(out IContemplable contemplableObj);

                // Verificamos si es un objeto válido y si es DIFERENTE al que ya procesamos en este zoom
                if (hasContemplable && contemplableObj.CanBeInteractedWith() && contemplableObj != lastContemplatedObject)
                {
                    if (lastContemplatedObject != null)
                    {
                        lastContemplatedObject.IsAbleToBeContemplate(true);
                    }

                    lastContemplatedObject = contemplableObj;

                    if (UIManager.Instance != null && UIManager.Instance.ContemplationCanvasHandler != null)
                    {
                        StartCoroutine(ContemplationCoroutine(contemplableObj));
                    }
                }
            }
            else
            {
                if (lastContemplatedObject != null)
                {
                    lastContemplatedObject.IsAbleToBeContemplate(true);
                    lastContemplatedObject = null;

                    // Opcional: Si miras a la nada, ¿quieres que se borre el texto?
                    // Según tu prompt, el texto se cancela "si el jugador deja de hacer zoom Y vuelve a hacerlo".
                    // Aquí el jugador sigue haciendo zoom pero miró a otro lado. 
                    // Si quieres que se borre al mirar a la nada manteniendo zoom, descomenta:
                    // UIManager.Instance.ContemplationCanvasHandler.HideContemplationCanvas();
                }
            }
        }

        private IEnumerator ContemplationCoroutine(IContemplable contemplableObj)
        {
            contemplableObj.IsAbleToBeContemplate(false);

            if (!contemplableObj.HasReachedMaxContemplations())
            {
                string contemplationText = contemplableObj.GetContemplationText();

                yield return StartCoroutine(UIManager.Instance.ContemplationCanvasHandler.ShowContemplationText(contemplationDelay, contemplationText));
            }
            else
            {
                // Si ya llegó al máximo, nos aseguramos que no se pueda volver a interactuar
                contemplableObj.IsAbleToBeContemplate(false);
            }
        }

        private void OnDisable()
        {
            if (UIManager.Instance != null && UIManager.Instance.ContemplationCanvasHandler != null)
            {
                UIManager.Instance.ContemplationCanvasHandler.HideContemplationCanvas();
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