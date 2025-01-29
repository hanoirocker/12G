namespace TwelveG.PlayerController
{
    using System;
    using System.Collections;
    using TwelveG.Localization;
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
        private LocalizationData localizationData;
        private bool canvasIsShowing;
        private InteractionTextSO canvasText;

        private void Awake()
        {
            localizationData = GetComponentInParent<LocalizationData>();
        }

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
                    canvasText = interactObj.RetrieveInteractionSO();
                    if(canvasText != null) { ShowUI(canvasText); }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (interactObj.Interact(this))
                        {
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

            if(observationTextSO == null) { yield return null; }

            onObservationCanvasShowText.Raise(this, observationTextSO);
            lastColliderInteractedWith.GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(3f);
            lastColliderInteractedWith.GetComponent<Collider>().enabled = true;
        }

        private void ShowUI(InteractionTextSO retrievedInteractionSO)
        {
            if (!canvasIsShowing)
            {
                onInteractionCanvasShowText.Raise(this, retrievedInteractionSO);
                canvasIsShowing = true;
            }
        }

        private void ChangeUI(IInteractable interactObj)
        {
            canvasText = interactObj.RetrieveInteractionSO();
            onInteractionCanvasShowText.Raise(this, canvasText);
        }

        private void HideUI()
        {
            // Ocultar el canvas de interaction si no se mira al objeto interactuable
            if (canvasIsShowing)
            {
                onInteractionCanvasControls.Raise(this, "HideText");
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