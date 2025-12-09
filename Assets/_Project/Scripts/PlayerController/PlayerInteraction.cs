namespace TwelveG.PlayerController
{
    using System;
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Raycast settings")]
        [SerializeField] private LayerMask interactableMask;
        [SerializeField, Range(0.5f, 2f)] private float interactRange = 1.2f;
        public Transform interactorSource;
        public Color raycastColor;

        [Header("EventsSO references")]
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;
        public GameEventSO onObservationCanvasShowText;

        private Collider lastColliderInteractedWith;
        private bool canvasIsShowing;
        private InteractionTextSO canvasText;

        private void Start()
        {
            lastColliderInteractedWith = null;
            canvasIsShowing = false;
        }

        private void Update()
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);

            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange, interactableMask))
            {
                HideUI();
                bool objectHasInteractableComponent = hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj);
                // Check if the object is interactable
                if (objectHasInteractableComponent && interactObj.CanBeInteractedWith(this))
                {
                    // Debug.Log($"{hitInfo.collider.gameObject.name}");
                    canvasText = interactObj.RetrieveInteractionSO(this);
                    if (canvasText != null) { ShowUI(canvasText); }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (interactObj.Interact(this))
                        {
                            HideUI();
                            return;
                        }
                        else
                        {
                            lastColliderInteractedWith = hitInfo.collider;
                            StartCoroutine(CancelInteractionAndShowObservation(interactObj, hitInfo.collider));
                        }
                    }
                }
            }
            else
            {
                HideUI();
            }
        }

        private IEnumerator CancelInteractionAndShowObservation(IInteractable interactObj, Collider collider)
        {
            ObservationTextSO observationTextSO = interactObj.GetFallBackText();
            PlayerContemplation contemplation = this.GetComponent<PlayerContemplation>();
            contemplation.enabled = false;

            // Calcular tiempo en base a texto para esperar antes de mostrar
            // el próximo texto de interacción
            string textToShow = Utils.TextFunctions.RetrieveObservationText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    observationTextSO
                );
            float timeToWait = Utils.TextFunctions.CalculateTextDisplayDuration(textToShow);

            if (observationTextSO == null) { yield return null; }

            onObservationCanvasShowText.Raise(this, observationTextSO);
            lastColliderInteractedWith.GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(timeToWait);
            lastColliderInteractedWith.GetComponent<Collider>().enabled = true;
            contemplation.enabled = true;
        }

        private void ShowUI(InteractionTextSO retrievedInteractionSO)
        {
            if (!canvasIsShowing && retrievedInteractionSO != null)
            {
                onInteractionCanvasShowText.Raise(this, retrievedInteractionSO);
                canvasIsShowing = true;
            }
        }

        private void HideUI()
        {
            // Ocultar el canvas de interaction si no se mira al objeto interactuable
            if (canvasIsShowing)
            {
                onInteractionCanvasControls.Raise(this, new HideText());
                canvasIsShowing = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = raycastColor;
            Vector3 direction = interactorSource.TransformDirection(Vector3.forward) * interactRange;
            Gizmos.DrawRay(interactorSource.position, direction);
        }
    }
}