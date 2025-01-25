namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.PlayerController;
    using TwelveG.Localization;
    using UnityEngine;

    public class PCHandler : MonoBehaviour, IInteractable
    {
        [Header("Sound settings")]
        [SerializeField] private AudioClip startUP = null;
        [SerializeField] private AudioClip click = null;

        [Header("Other components settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject contemplableObject;
        [SerializeField] private List<GameObject> pcScreens = new List<GameObject>();

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        [Header("EventsSO references")]
        public GameEventSO onImageCanvasControls;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;
        public GameEventSO onControlCanvasControls;
        public GameEventSO onObservationCanvasShowText;
        public GameEventSO onPlayerControls;
        public GameEventSO onVirtualCamerasControl;

        [Header("Other eventsSO references")]
        public GameEventSO onPC;

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return true;
        }

        public string GetFallBackText(string currentLanguage)
        {
            throw new System.NotImplementedException();
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return interactionTextsSO;
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            StartCoroutine(UsePC(playerCamera));
            return true;
        }

        private IEnumerator UsePC(PlayerInteraction playerCamera)
        {
            // Aca avisamos al LostSignal que comenzamos de usar la PC
            onPC.Raise(this, null);

            onImageCanvasControls.Raise(this, "FadeOutImage");
            yield return new WaitForSeconds(1f);

            onVirtualCamerasControl.Raise(this, "EnablePCVC");

            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            onPlayerControls.Raise(this, "EnableHeadLookAround");

            onImageCanvasControls.Raise(this, "FadeInImage");

            GetComponent<SphereCollider>().enabled = false;

            // Change to loading windows screen
            yield return new WaitForSeconds(3f);
            pcScreens[0].SetActive(false);
            pcScreens[1].SetActive(true);

            // Change to loading desktop screen
            yield return new WaitForSeconds(5f);
            pcScreens[1].SetActive(false);
            pcScreens[2].SetActive(true);
            audioSource.PlayOneShot(startUP);

            yield return new WaitUntil(() => !audioSource.isPlaying);
            yield return new WaitForSeconds(3f);
            audioSource.PlayOneShot(click);
            yield return new WaitForSeconds(1f);
            audioSource.PlayOneShot(click);

            // Cambiar a pantalla de discordia not connecting
            yield return new WaitForSeconds(3f);
            pcScreens[2].SetActive(false);
            pcScreens[3].SetActive(true);

            yield return new WaitForSeconds(8f);
            onObservationCanvasShowText.Raise(this, "Que raro, la habran ...");
            yield return new WaitForSeconds(4f);
            onObservationCanvasShowText.Raise(this, "Ok, a lo importante");

            yield return new WaitForSeconds(4.5f);
            audioSource.PlayOneShot(click);
            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(click);

            // Cambiar a pantalla steam not connecting
            yield return new WaitForSeconds(2f);
            pcScreens[3].SetActive(false);
            pcScreens[4].SetActive(true);

            yield return new WaitForSeconds(10f);
            onObservationCanvasShowText.Raise(this, "..........................");

            // Cambiar a pantalla steam not connected
            yield return new WaitForSeconds(7f);
            pcScreens[4].SetActive(false);
            pcScreens[5].SetActive(true);

            yield return new WaitForSeconds(3f);
            onObservationCanvasShowText.Raise(this, "Ok parece que no.....");

            yield return new WaitForSeconds(3f);

            onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onImageCanvasControls.Raise(this, "FadeOutImage2");
            yield return new WaitForSeconds(2f);

            onInteractionCanvasControls.Raise(this, "HideText");

            onControlCanvasControls.Raise(this, "HideControlCanvas");

            onVirtualCamerasControl.Raise(this, "DisablePCVC");

            onPlayerControls.Raise(this, "DisableHeadLookAround");

            onImageCanvasControls.Raise(this, "FadeInImage2");

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

            // Aca avisamos al LostSignal que terminamos de usar la PC
            onPC.Raise(this, null);

            this.enabled = false;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new NotImplementedException();
        }

        private void OnDisable()
        {
            // DUDA: Si la PC no se volverá a usar en ningún momento del juego, podriamos apagar acá el Game Event Listener.

            // Se activa el objecto Contemplable 
            contemplableObject.SetActive(true);

            // Se desactiva el objeto Interactuable que posee este script
            this.gameObject.SetActive(false);
        }
    }
}