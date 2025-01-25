namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cinemachine;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class BackpackHandler : MonoBehaviour, IInteractable
    {
        [Header("Text Settings")]
        [SerializeField] private List<string> searchingTexts = new List<string>();

        [Header("Sound settings")]
        [SerializeField] private List<AudioClip> searchingSounds = null;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        public GameEventSO onObservationCanvasShowText;
        public GameEventSO onPlayerControls;
        public GameEventSO onVirtualCamerasControl;

        private AudioSource audioSource;
        private Animation backpackAnimation = null;
        private bool canBeInteractedWith = true;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCameraObject)
        {
            return canBeInteractedWith;
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return interactionTextsSO;
        }

        public bool Interact(PlayerInteraction playerCameraObject)
        {
            if (canBeInteractedWith)
            {
                StartCoroutine(CheckBag(playerCameraObject));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator CheckBag(PlayerInteraction playerCameraObject)
        {
            canBeInteractedWith = false;

            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            onVirtualCamerasControl.Raise(this, "EnableBackpackVC");

            yield return new WaitUntil(() => backpackAnimation != null);
            backpackAnimation.PlayQueued("Night - Backpack - Search 1");

            audioSource.PlayOneShot(searchingSounds[0]);
            yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(1f);
            onObservationCanvasShowText.Raise(this, "Un par de forros sin usar");
            yield return new WaitForSeconds(1f);

            backpackAnimation.PlayQueued("Night - Backpack - Search 2");

            audioSource.PlayOneShot(searchingSounds[1]);
            yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(1f);
            onObservationCanvasShowText.Raise(this, "Caramelos");
            yield return new WaitForSeconds(1f);

            backpackAnimation.PlayQueued("Night - Backpack - Search 3");

            audioSource.PlayOneShot(searchingSounds[2]);
            yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(1f);
            onObservationCanvasShowText.Raise(this, "Definitivamente no");
            yield return new WaitForSeconds(1f);


            onVirtualCamerasControl.Raise(this, "DisableBackpackVC");

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

        }

        public void SetVCAnimationComponent(Component sender, object data)
        {
            if (data != null)
            {
                CinemachineVirtualCamera backpackVC = (CinemachineVirtualCamera)data;
                backpackAnimation = backpackVC.GetComponent<Animation>();
            }
        }

        public void DestroyInteractiveBackpack(Component sender, object data)
        {
            Destroy(gameObject);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject)
        {
            throw new System.NotImplementedException();
        }

        public string GetFallBackText(string currentLanguage)
        {
            throw new NotImplementedException();
        }
    }
}