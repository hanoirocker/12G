using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class MicrowaveHandler : MonoBehaviour, IInteractable
    {
        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType = null;

        [Header("Pickable to activate")]
        [SerializeField] private GameObject pickableHeatedPizza = null;

        [Header("Audio settings")]
        [SerializeField] private AudioClip heatingSound = null;
        [SerializeField, Range(0f, 1f)] private float heatingSoundVolume = 0.5f;
        [SerializeField] private AudioClip finishedHeatingSound = null;
        [SerializeField, Range(0f, 1f)] private float finishedHeatingSoundVolume = 0.5f;
        [SerializeField] private AudioClip managePlate = null;
        [SerializeField, Range(0f, 1f)] private float managePlateVolume = 0.5f;

        [Header("Other components settings")]
        [SerializeField] private RotativeDrawerHandler rotativeDrawerHandler;
        [SerializeField] private Renderer doorRenderer;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;
        [SerializeField] private InteractionTextSO interactionTextsSO_heatPizza;

        private List<String> playerItems = new List<String>();
        private AudioSource audioSource;
        private AudioSourceState audioSourceState;
        private Material doorMaterial;

        void Start()
        {
            if (doorRenderer != null)
            {
                doorMaterial = doorRenderer.material;
            }
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            if (rotativeDrawerHandler.DoorIsOpen() && VerifyIfPlayerCanInteract(playerCamera))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            throw new System.NotImplementedException();
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            if (rotativeDrawerHandler.DoorIsOpen())
            {
                if (VerifyIfPlayerCanInteract(playerCamera))
                {
                    return interactionTextsSO_heatPizza;
                }
                else
                {
                    return interactionTextsSO_open;
                }
            }
            else
            {
                return interactionTextsSO_close;
            }
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            bool playerHasRequiredItems = VerifyIfPlayerCanInteract(playerCamera);

            if (playerHasRequiredItems)
            {
                StartCoroutine(HeatPizza(playerCamera));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator HeatPizza(PlayerInteraction playerCamera)
        {
            RemoveUsedItems(playerCamera);

            (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, 1f);
            audioSource.clip = managePlate;
            audioSource.volume = managePlateVolume;
            audioSource.Play();
            GetComponentInChildren<RotativeDrawerHandler>().Interact(playerCamera);

            yield return new WaitUntil(() => !rotativeDrawerHandler.DoorIsOpen());

            // Comentar siguiente bloque al testear evento
            if (heatingSound)
            {
                GetComponentInChildren<SphereCollider>().enabled = false;
                audioSource.clip = heatingSound;
                audioSource.volume = heatingSoundVolume;
                audioSource.Play();
                doorMaterial.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(heatingSound.length * 0.5f);
                doorMaterial.DisableKeyword("_EMISSION");
            }

            if (finishedHeatingSound)
            {
                audioSource.clip = finishedHeatingSound;
                audioSource.volume = finishedHeatingSoundVolume;
                audioSource.Play();
                yield return new WaitForSeconds(finishedHeatingSound.length);
                GetComponentInChildren<SphereCollider>().enabled = true;
            }

            pickableHeatedPizza.SetActive(true);

            yield return new WaitUntil(() => rotativeDrawerHandler.DoorIsOpen());

            pickableHeatedPizza.GetComponent<Collider>().enabled = true;

            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            audioSource = null;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            playerItems = playerCamera.GetComponentInChildren<PlayerInventory>().returnPickedItems();

            bool playerHasNeededItems = objectsNeededType.All(item => playerItems.Contains(item.ToString()));

            return playerHasNeededItems;
        }

        private void RemoveUsedItems(PlayerInteraction playerCamera)
        {
            // Accede al inventario del jugador y remueve los objetos necesarios para la tarea
            var playerInventory = playerCamera.GetComponentInChildren<PlayerInventory>();
            foreach (var itemNeeded in objectsNeededType)
            {
                playerInventory.RemoveItem(itemNeeded);
            }
        }
    }
}