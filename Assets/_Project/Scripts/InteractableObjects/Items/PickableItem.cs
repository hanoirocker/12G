using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class PickableItem : MonoBehaviour, IItem
    {
        [Header("Item settings")]
        [Space]
        [SerializeField] private ItemType itemType;
        public bool canBePicked;
        public bool triggerEventWhenPicked = false;

        [Header("Audio settings")]
        [Space]
        [SerializeField] private AudioClip pickItemSound;
        [SerializeField, Range(0f, 1f)] private float pickItemSoundVolume = 0.7f;

        [Header("Interaction Texts SO")]
        [Space]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        [Header("Event SO references")]
        [Space]
        [SerializeField] private GameEventSO eventToTriggerWhenItemPicked;


        public void AllowToBePicked(Component sender, object data)
        {
            canBePicked = (bool)data;
        }

        public bool CanBePicked()
        {
            return canBePicked;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return interactionTextsSO;
        }

        public ItemType GetItemType()
        {
            return itemType;
        }

        // Funcion principal
        public void TakeItem()
        {
            StartCoroutine(TakeItemCoroutine());
        }

        private IEnumerator TakeItemCoroutine()
        {
            if (triggerEventWhenPicked)
            {
                eventToTriggerWhenItemPicked.Raise(this, null);

            }
            if (pickItemSound != null)
            {
                (AudioSource audioSource, AudioSourceState audioSourceState) =
                    AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                        gameObject.transform, 
                        pickItemSoundVolume
                );

                audioSource.PlayOneShot(pickItemSound);
                yield return new WaitUntil(() => !audioSource.isPlaying);
                AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
                audioSource = null;
            }

            gameObject.SetActive(false);
        }
    }
}