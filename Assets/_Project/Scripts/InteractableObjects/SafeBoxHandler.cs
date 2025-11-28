namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class SafeBoxHandler : MonoBehaviour, IInteractable
    {
        [Header("Object Settings")]
        [SerializeField] private GameObject doorInteractable;
        [SerializeField] private GameObject walkieTalkie;
        [SerializeField] private bool doorIsLocked = true;

        [Header("Events SO references")]
        [SerializeField] private GameEventSO safeBoxNoteCanBeExamine;

        [Header("Text Settings")]
        [SerializeField] private ObservationTextSO observationFallback;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_try;
        [SerializeField] private InteractionTextSO interactionTextsSO_interact;

        [Header("Testing")]
        public bool isTestingMode = false;

        private SafeBoxKeyboardHandler safeBoxKeyboardHandler;
        private bool canBeInteractedWith = true;
        private int lockedIndex = 0;

        private void Awake()
        {
            safeBoxKeyboardHandler = GetComponent<SafeBoxKeyboardHandler>();
        }

        void Start()
        {
            if (!doorIsLocked)
            {
                UnlockSafeBox();
            }
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return canBeInteractedWith;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return GetDoorTextForCanvas();
        }

        private InteractionTextSO GetDoorTextForCanvas()
        {
            if (doorIsLocked)
            {
                if (isTestingMode) { return interactionTextsSO_interact; }
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
                if (lockedIndex > 0 || isTestingMode)
                {
                    StartCoroutine(InteractWithKeyCode(playerCamera));
                    return true;
                }
                else
                {
                    lockedIndex += 1;
                    safeBoxNoteCanBeExamine.Raise(this, true);
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
            safeBoxKeyboardHandler.enabled = true;
            yield return new WaitUntil(() => !safeBoxKeyboardHandler.enabled);
            canBeInteractedWith = true;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            return CanBeInteractedWith(playerCamera);
        }

        public ObservationTextSO GetFallBackText()
        {
            return observationFallback;
        }

        public void UnlockSafeBox()
        {
            doorIsLocked = false;
            canBeInteractedWith = false;
            GetComponent<BoxCollider>().enabled = false;

            doorInteractable.GetComponent<RotativeDrawerHandler>().enabled = true;
            doorInteractable.GetComponent<Collider>().enabled = true;
            walkieTalkie.GetComponent<PickableItem>().canBePicked = true;
        }
    }
}