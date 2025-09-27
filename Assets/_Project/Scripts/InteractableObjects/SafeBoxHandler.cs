namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class SafeBoxHandler : MonoBehaviour, IInteractable
    {
        [Header("Object Settings")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsLocked = true;

        [Header("Text Settings")]
        [SerializeField] private ObservationTextSO observationFallback;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_try;
        [SerializeField] private InteractionTextSO interactionTextsSO_interact;

        private bool canBeInteractedWith = true;
        private int lockedIndex = 0;

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return canBeInteractedWith;
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return GetDoorTextForCanvas();
        }

        private InteractionTextSO GetDoorTextForCanvas()
        {
            if (doorIsLocked)
            {
                if (lockedIndex == 0) { return interactionTextsSO_try; }
                else { return interactionTextsSO_interact; }
            }
            else
            {
                return null;
            }
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            if (doorIsLocked)
            {
                if (lockedIndex > 0)
                {
                    StartCoroutine(InteractWithKeyCode(playerCamera));
                    return true;
                }
                else
                {
                    lockedIndex += 1;
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private IEnumerator InteractWithKeyCode(PlayerInteraction playerCamera)
        {
            canBeInteractedWith = false;
            Debug.Log("Reproduciendo interacción");
            yield return new WaitForSeconds(4f);
            Debug.Log("Interacción finalizada, abriendo caja");
            door.GetComponent<RotativeDrawerHandler>().enabled = true;
            door.GetComponent<Collider>().enabled = true;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            return CanBeInteractedWith(playerCamera);
        }

        public ObservationTextSO GetFallBackText()
        {
            return observationFallback;
        }
    }
}