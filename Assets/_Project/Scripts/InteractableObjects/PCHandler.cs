using System;
using System.Collections;
using System.Collections.Generic;
using TwelveG.PlayerController;
using TwelveG.Localization;
using UnityEngine;
using TwelveG.AudioController;
using TwelveG.GameController;

namespace TwelveG.InteractableObjects
{
    public class PCHandler : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private SphereCollider turnOnCollider = null;
        [SerializeField] private SphereCollider turnOffCollider = null;
        [SerializeField] private GameObject resonanceZone = null;
        [SerializeField] private GameObject contemplableObject;
        [SerializeField] private List<GameObject> pcScreens = new List<GameObject>();

        [Space]
        [Header("Sound settings")]
        [SerializeField] private AudioClip logInClip = null;
        [SerializeField] private AudioClip click = null;
        [SerializeField] private AudioClip logOutClip = null;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.5f;

        [Space]
        [Header("Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;
        [SerializeField] private InteractionTextSO turnOff;
        [SerializeField] private List<ObservationTextSO> observationsTextsSOs;

        [Header("Other eventsSO references")]
        public GameEventSO onPC;

        private bool canBeInteractedWith = true;
        private bool isTurnedOn = false;

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return canBeInteractedWith;
        }

        public ObservationTextSO GetFallBackText()
        {
            throw new System.NotImplementedException();
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return isTurnedOn ? turnOff : interactionTextsSO;
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            if (!isTurnedOn)
            {
                StartCoroutine(UsePC(playerCamera));
                return true;
            }
            else
            {
                StartCoroutine(TurnOffPC(playerCamera));
                return false;
            }
        }

        private IEnumerator TurnOffPC(PlayerInteraction playerCamera)
        {
            if (logOutClip == null)
            {
                Debug.LogWarning("No shutdown clip assigned to PCHandler!");
                yield break;
            }

            playerCamera.GetComponentInParent<PlayerHandler>().PlayerControls(this, new EnablePlayerControllers(false));

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(click);
            yield return new WaitForSeconds(1f);
            audioSource.PlayOneShot(click);
            yield return new WaitForSeconds(0.7f);
            audioSource.PlayOneShot(click);
            yield return new WaitForSeconds(1f);
            audioSource.PlayOneShot(logOutClip);
            yield return new WaitForSeconds(0.5f);

            playerCamera.GetComponentInParent<PlayerHandler>().PlayerControls(this, new EnablePlayerControllers(true));
            yield return new WaitUntil(() => !audioSource.isPlaying);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);

            pcScreens[5].SetActive(false);
            pcScreens[0].SetActive(true);
            isTurnedOn = false;
            resonanceZone.SetActive(false);
            turnOffCollider.enabled = false;
            turnOnCollider.enabled = true;
            canBeInteractedWith = false;
        }

        private IEnumerator UsePC(PlayerInteraction playerCamera)
        {
            // Aca avisamos al LostSignal que comenzamos de usar la PC
            resonanceZone.SetActive(true);
            onPC.Raise(this, null);
            isTurnedOn = true;

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

            gameObject.GetComponent<SphereCollider>().enabled = false;

            // Espera el tiempo que tarda en hacer el fadeout + fadein para cambiar
            // a la camara VCPC desde LostSignalEvent
            yield return new WaitForSeconds(2f);


            // Change to loading windows screen
            yield return new WaitForSeconds(3f);
            pcScreens[0].SetActive(false);
            pcScreens[1].SetActive(true);

            // Change to loading desktop screen
            yield return new WaitForSeconds(5f);
            pcScreens[1].SetActive(false);
            pcScreens[2].SetActive(true);
            audioSource.PlayOneShot(logInClip);

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

            // Qué raro ¿La habrán cagado con la última actualización?
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationsTextsSOs[0]);
            yield return new WaitForSeconds(4f);
            // Ok, a lo importante.
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationsTextsSOs[1]);

            yield return new WaitForSeconds(4.5f);
            audioSource.PlayOneShot(click);
            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(click);

            // Cambiar a pantalla steam not connecting
            yield return new WaitForSeconds(2f);
            pcScreens[3].SetActive(false);
            pcScreens[4].SetActive(true);

            yield return new WaitForSeconds(10f);
            // ..........................
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationsTextsSOs[2]);

            // Cambiar a pantalla steam not connected
            yield return new WaitForSeconds(7f);
            pcScreens[4].SetActive(false);
            pcScreens[5].SetActive(true);

            yield return new WaitForSeconds(3f);
            // Parece que no hay chances que algo salga bien hoy. Fibertol me re contra defeco en vos.
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationsTextsSOs[3]);

            yield return new WaitForSeconds(3f);

            // Aca avisamos al LostSignal que ya no usamos mas la PC
            onPC.Raise(this, null);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);

            turnOnCollider.enabled = false;
            turnOffCollider.enabled = true;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new NotImplementedException();
        }

        private void OnDisable()
        {
            // Se activa el objecto Contemplable 
            contemplableObject.SetActive(true);

            // Se desactiva el objeto Interactuable que posee este script
            this.gameObject.SetActive(false);
        }
    }
}