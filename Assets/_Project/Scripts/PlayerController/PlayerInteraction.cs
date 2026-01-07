using System;
using System.Collections;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.PlayerController
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private LayerMask interactableMask;
        [SerializeField, Range(0.5f, 2f)] private float interactRange = 1.2f;

        [Header("Text References")]
        [SerializeField] private ObservationTextSO[] cantInteractDueToHandsTextSO;

        public Transform interactorSource;
        public Color raycastColor;

        private Collider lastColliderInteractedWith;
        private bool canvasIsShowing;
        private InteractionTextSO canvasText;
        private PlayerInventory playerInventory;

        void Awake()
        {
            playerInventory = GetComponentInChildren<PlayerInventory>();
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
                    // Debug.Log($"{hitInfo.collider.gameObject.name}");
                    canvasText = interactObj.RetrieveInteractionSO(this);
                    if (canvasText != null) { ShowUI(canvasText); }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (playerInventory.PlayerHasBothHandsOccupied())
                        {
                            lastColliderInteractedWith = hitInfo.collider;
                            StartCoroutine(CancelInteractionAndShowObservation(interactObj, true));
                            return;
                        }
                        if (interactObj.Interact(this))
                        {
                            HideUI();
                            return;
                        }
                        else
                        {
                            lastColliderInteractedWith = hitInfo.collider;
                            StartCoroutine(CancelInteractionAndShowObservation(interactObj, false));
                        }
                    }
                }
            }
            else
            {
                HideUI();
            }
        }

        private IEnumerator CancelInteractionAndShowObservation(IInteractable interactObj, bool hasOccupiedHands)
        {
            ObservationTextSO observationTextSO;
            float timeUntilShown;

            if (hasOccupiedHands)
            {
                observationTextSO = cantInteractDueToHandsTextSO[UnityEngine.Random.Range(0, cantInteractDueToHandsTextSO.Length)];
                timeUntilShown = 0.25f;
            }
            else
            {
                (observationTextSO, timeUntilShown) = interactObj.GetFallBackText();
            }

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

            lastColliderInteractedWith.GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(timeUntilShown);
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSO);
            yield return new WaitForSeconds(timeToWait);
            lastColliderInteractedWith.GetComponent<Collider>().enabled = true;
            contemplation.enabled = true;
        }

        private void ShowUI(InteractionTextSO retrievedInteractionSO)
        {
            if (!canvasIsShowing && retrievedInteractionSO != null)
            {
                GameEvents.Common.onInteractionCanvasShowText.Raise(this, retrievedInteractionSO);
                canvasIsShowing = true;
            }
        }

        private void HideUI()
        {
            // Ocultar el canvas de interaction si no se mira al objeto interactuable
            if (canvasIsShowing)
            {
                GameEvents.Common.onInteractionCanvasControls.Raise(this, new HideText());
                canvasIsShowing = false;
            }
        }

        private void OnDisable()
        {
            HideUI();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = raycastColor;
            Vector3 direction = interactorSource.TransformDirection(Vector3.forward) * interactRange;
            Gizmos.DrawRay(interactorSource.position, direction);
        }
    }
}