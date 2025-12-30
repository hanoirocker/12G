using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class CleanBirdsHandler : MonoBehaviour, IInteractable
    {
        [Header("Objects to Modify")]
        [SerializeField] private List<GameObject> objectsToModify = new List<GameObject>();

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Resulting Objects")]
        [SerializeField] private List<ItemType> resultingObjectsType;

        [Header("Visual settings")]
        [SerializeField] private bool fadesImage = false;

        [Header("Audio settings")]
        [SerializeField] private AudioClip cleaningSound = null;
        [SerializeField] [Range(0f, 1f)] private float cleaningSoundVolume = 0.8f;

        [Header("EventsSO references")]
        public GameEventSO cleanZoomBird;
        public GameEventSO onPlayerControls;
        public GameEventSO onImageCanvasControls;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        [Header("Fallback Texts SO")]
        [SerializeField] private ObservationTextSO observationTextSO_hasBroom;
        [SerializeField] private ObservationTextSO observationTextSO_hasBag;
        [SerializeField] private ObservationTextSO observationTextSO_noneItem;
        [SerializeField, Range(0f, 5f)] private float timeBeforeShowingFallbackText = 0f;

        List<String> playerItems = new List<String>();
        private AudioSourceState audioSourceState;
        private AudioSource audioSource;
        private bool canBeInteractedWith = true;
        private bool resultsInConsequentObjects = false;

        void Start()
        {
            VerifyResultingObjects();
        }

        // Verifica si la lista de objetos resultantes no es nula
        private void VerifyResultingObjects()
        {
            if (resultingObjectsType != null && resultingObjectsType.Count > 0)
            {
                resultsInConsequentObjects = true;
            }
            else
            {
                return;
            }
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return canBeInteractedWith;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            if (canBeInteractedWith)
            {
                return interactionTextsSO;
            }
            else
            {
                return null;
            }
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            bool playerCanCleanBirds = VerifyIfPlayerCanInteract(playerCamera);

            if (playerCanCleanBirds)
            {
                canBeInteractedWith = false;
                StartCoroutine(CleanBirds(objectsToModify, playerCamera));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator CleanBirds(List<GameObject> objectsToModify, PlayerInteraction playerCamera)
        {
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(this.gameObject.transform, cleaningSoundVolume);
            audioSource.PlayOneShot(cleaningSound);

            if (fadesImage)
            {
                GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitUntil(() => !audioSource.isPlaying);

            RemoveUsedItems(playerCamera);

            // Apaga el mesh renderer para simular que desaparecieron el ave y los vidrios
            foreach (GameObject gameObject in objectsToModify)
            {
                if (gameObject.GetComponent<SkinnedMeshRenderer>())
                {
                    gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }
                else
                {
                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            if (fadesImage)
            {
                GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
                yield return new WaitForSeconds(1f);
            }

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));

            cleanZoomBird.Raise(this, null);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            audioSource = null;

            Destroy(this.gameObject);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            playerItems = playerCamera.GetComponentInChildren<PlayerInventory>().returnPickedItems();

            bool allItemsPresent = objectsNeededType.All(item => playerItems.Contains(item.ToString()));

            return allItemsPresent;
        }

        private void RemoveUsedItems(PlayerInteraction playerCamera)
        {
            // Accede al inventario del jugador y remueve los objetos necesarios para la tarea
            var playerInventory = playerCamera.GetComponentInChildren<PlayerInventory>();
            foreach (var itemNeeded in objectsNeededType)
            {
                playerInventory.RemoveItem(itemNeeded);
            }
            // Agrega el ItemType resultante de la interacción al inventario del usuario.
            // Dependiendo del tipo de item, `RemoveItem` en PlayerIventory hará cosas distantas.
            if (resultsInConsequentObjects)
            {
                foreach (var resultingItem in resultingObjectsType)
                {
                    playerInventory.AddItem(resultingItem);
                }
            }
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            if (!playerItems.Contains(ItemType.EmptyTrashBag.ToString()) && playerItems.Contains(ItemType.Broom.ToString()))
            {
                //"Aun me falta una bolsa, creo que habia un rollo en el cajon de la cocina...";
                return (observationTextSO_hasBroom, timeBeforeShowingFallbackText);
            }
            if (playerItems.Contains(ItemType.EmptyTrashBag.ToString()) && !playerItems.Contains(ItemType.Broom.ToString()))
            {
                //"Aun me falta la escoba";
                return (observationTextSO_hasBag, timeBeforeShowingFallbackText);
            }
            else
            {
                //"La escoba de la cocina y una bolsa me vendrian bien para esto...";
                return (observationTextSO_noneItem, timeBeforeShowingFallbackText);
            }
        }
    }
}