namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cinemachine;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
  using TwelveG.Utils;
  using UnityEngine;

    public class BackpackHandler : MonoBehaviour, IInteractable
    {
        [Header("Sound settings")]
        [SerializeField] private List<AudioClip> searchingSounds = null;

        [Header("Interaction Texts SO's")]
        [SerializeField] private InteractionTextSO interactionTextsSO;
        [SerializeField] private List<ObservationTextSO> searchingTexts = new List<ObservationTextSO>();

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onPlayerControls;
        [SerializeField] private GameEventSO onVirtualCamerasControl;
        [SerializeField] private GameEventSO onMainCameraSettings;

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

            onPlayerControls.Raise(this, new TogglePlayerCapsule(false));

            onMainCameraSettings.Raise(this, "EasyInOut2");

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, true));

            yield return new WaitUntil(() => backpackAnimation != null);
            backpackAnimation.PlayQueued("Night - Backpack - Search 1");

            audioSource.PlayOneShot(searchingSounds[0]);
            yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(1f);
            // Un par de forros ( sin usar obviamente )
            onObservationCanvasShowText.Raise(this, searchingTexts[0]);
            yield return new WaitForSeconds(1f);

            backpackAnimation.PlayQueued("Night - Backpack - Search 2");

            audioSource.PlayOneShot(searchingSounds[1]);
            yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(1f);
            // Caramelos cubiertos de pelos?
            onObservationCanvasShowText.Raise(this, searchingTexts[1]);
            yield return new WaitForSeconds(1f);

            backpackAnimation.PlayQueued("Night - Backpack - Search 3");

            audioSource.PlayOneShot(searchingSounds[2]);
            yield return new WaitUntil(() => !audioSource.isPlaying);

            yield return new WaitForSeconds(1f);
            // Definitivamente no está aca dentro
            onObservationCanvasShowText.Raise(this, searchingTexts[2]);
            yield return new WaitForSeconds(1f);

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, false));

            onPlayerControls.Raise(this, new TogglePlayerCapsule(true));

            // Esperar hasta que transicione de cámaras para resetear
            // el modo de transición
            yield return new WaitForSeconds(2.5f);
            onMainCameraSettings.Raise(this, "Cut");

            // Si no mal recuerdo, la última vez que revisé mi celular fue mientras miraba la TV hoy por la mañana
            yield return new WaitForSeconds(3f);
            onObservationCanvasShowText.Raise(this, searchingTexts[3]);
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

        public ObservationTextSO GetFallBackText()
        {
            throw new NotImplementedException();
        }
    }
}