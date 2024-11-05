namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TwelveG.PlayerController;
    using UnityEngine;

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

        [Header("EventsSO references")]
        public GameEventSO cleanZoomBird;
        public GameEventSO onPlayerControls;
        public GameEventSO onImageCanvasControls;

        List<String> playerItems = new List<String>();
        private AudioSource audioSource;
        private bool canBeInteractedWith = true;
        private bool resultsInConsequentObjects = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

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

        public string GetInteractionPrompt()
        {
            if(canBeInteractedWith)
            {
                return "LOCALIZATION!";
            }
            else
            {
                return "";
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
            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            audioSource.Play();

            if (fadesImage)
            {
                onImageCanvasControls.Raise(this, "FadeOutImage");
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
                onImageCanvasControls.Raise(this, "FadeInImage");
                yield return new WaitForSeconds(1f);
            }

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

            cleanZoomBird.Raise(this, null);

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

        public string GetFallBackText()
        {
            // TODO: Agregar fallbacktexts SO para retornar según codigo del lenguage
            if (!playerItems.Contains(ItemType.EmptyTrashBag.ToString()) && playerItems.Contains(ItemType.Broom.ToString()))
            {
                return "Aun me falta una bolsa, creo que habia un rollo en el cajon de la cocina...";
            }
            if (playerItems.Contains(ItemType.EmptyTrashBag.ToString()) && !playerItems.Contains(ItemType.Broom.ToString()))
            {
                return "Aun me falta la escoba";
            }
            else
            {
                return "La escoba de la cocina y una bolsa me vendrian bien para esto...";
            }
        }
    }
}