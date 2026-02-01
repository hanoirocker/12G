using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class BackpackHandler : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField, Range(2, 4)] private int cameraTrasitionTime = 2;
        
        [Space(5)]
        [Header("Animations")]
        [SerializeField] private AnimationClip searchAnimation1;
        [SerializeField] private AnimationClip searchAnimation2;
        [SerializeField] private AnimationClip searchAnimation3;

        [Space(5)]
        [Header("Audio settings")]
        [SerializeField] private List<AudioClip> searchingSounds = null;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;
        
        [Space(5)]
        [Header("Interaction Texts SO's")]
        [SerializeField] private InteractionTextSO interactionTextsSO;
        [SerializeField] private List<ObservationTextSO> searchingTexts = new List<ObservationTextSO>();

        private Animation backpackAnimation = null;
        private bool canBeInteractedWith = true;

        public bool CanBeInteractedWith(PlayerInteraction playerCameraObject)
        {
            return canBeInteractedWith;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
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

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, true));
            yield return new WaitForSeconds(cameraTrasitionTime);

            yield return new WaitUntil(() => backpackAnimation != null);
            backpackAnimation.PlayQueued(searchAnimation1.name);

            audioSource.PlayOneShot(searchingSounds[0]);
            yield return new WaitForSeconds(searchingSounds[0].length);

            yield return new WaitForSeconds(1f);
            // Un par de forros ( sin usar obviamente )
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(searchingTexts[0]);
            yield return new WaitForSeconds(1f);

            backpackAnimation.PlayQueued(searchAnimation2.name);

            audioSource.PlayOneShot(searchingSounds[1]);
            yield return new WaitForSeconds(searchingSounds[1].length);

            yield return new WaitForSeconds(1f);
            // Caramelos cubiertos de pelos?
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(searchingTexts[1]);
            yield return new WaitForSeconds(1f);

            backpackAnimation.PlayQueued(searchAnimation3.name);

            audioSource.PlayOneShot(searchingSounds[2]);
            yield return new WaitForSeconds(searchingSounds[2].length);

            yield return new WaitForSeconds(1f);
            // Definitivamente no está aca dentro
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(searchingTexts[2]);
            yield return new WaitForSeconds(1f);


            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Backpack, false));
            yield return new WaitForSeconds(cameraTrasitionTime);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));

            // Si no mal recuerdo, la última vez que revisé mi celular fue mientras miraba la TV hoy por la mañana
            yield return new WaitForSeconds(3f);
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(searchingTexts[3]);

            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            audioSource = null;
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

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            throw new NotImplementedException();
        }
    }
}