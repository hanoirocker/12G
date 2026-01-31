using TwelveG.PlayerController;
using System.Collections;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.AudioController;

namespace TwelveG.InteractableObjects
{
    public class RotativeDoorHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpen;
        [SerializeField] Animation doorPickAnimation;
        [SerializeField] private bool canBeClosed = true;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        [Header("Audio settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField] private AudioClip hardClosingDoorSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float hardClipsVolume = 1f;

        [Header("Movement Settings")]
        [SerializeField, Range(0.1f, 1f)] private float quickToggleDuration = 0.45f;

        [Tooltip("Define la velocidad de apertura. Recomendado: Ease-Out (Rápido al inicio, suave al final).")]
        [SerializeField] private AnimationCurve openingMovementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool isMoving;
        private Quaternion initialRotation;

        private void Start()
        {
            isMoving = false;
            initialRotation = door.transform.localRotation;
        }

        public bool IsDoorOpen()
        {
            return doorIsOpen;
        }

        private void ToggleDoor()
        {
            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(0, 90, 0);
            StartCoroutine(RotateDoor(targetRotation));

            if (doorPickAnimation && !doorIsOpen)
            {
                doorPickAnimation.Play();
            }
        }

        private IEnumerator RotateDoor(Quaternion targetRotation)
        {
            isMoving = true;
            bool isOpeningAction = !doorIsOpen;

            AudioClip clip = isOpeningAction ? openingDoorSound : closingDoorSound;

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                door.transform,
                clipsVolume
            );

            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.8f, 1.4f);
                audioSource.clip = clip;
                audioSource.Play();
            }

            // Calculamos duración basada en el audio
            // Agregamos un pequeño clamp para evitar duraciones de 0 si el clip es null
            float coroutineDuration = (clip != null && audioSource != null)
                ? AudioUtils.CalculateDurationWithPitch(clip, audioSource.pitch)
                : 1.0f;

            float elapsedTime = 0f;
            Quaternion startRotation = door.transform.localRotation;

            while (elapsedTime < coroutineDuration)
            {
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / coroutineDuration);

                // Si estamos abriendo, usamos la curva. Si cerramos, lineal (o podríamos poner otra curva EVENTUALMENTE).
                float evaluationT = isOpeningAction ? openingMovementCurve.Evaluate(t) : t;

                door.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, evaluationT);

                yield return null;
            }

            door.transform.localRotation = targetRotation;
            doorIsOpen = !doorIsOpen;

            if (audioSource != null)
            {
                AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            }

            isMoving = false;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            if (doorIsOpen && !canBeClosed) return false;

            ToggleDoor();
            return true;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return !isMoving;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            if (!canBeClosed && doorIsOpen)
            {
                return null;
            }

            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_open;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            throw new System.NotImplementedException();
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            return (null, 0f);
        }

        public void StrongClosing()
        {
            if (!doorIsOpen) return;

            Quaternion targetRotation = initialRotation;
            StartCoroutine(StrongRotationCoroutine(targetRotation));
        }

        private IEnumerator StrongRotationCoroutine(Quaternion targetRotation)
        {
            isMoving = true;

            float elapsedTime = 0f;
            Quaternion startRotation = door.transform.localRotation;

            while (elapsedTime < quickToggleDuration)
            {
                door.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / quickToggleDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            door.transform.localRotation = targetRotation;
            doorIsOpen = false;

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                door.transform,
                hardClipsVolume
            );

            if (hardClosingDoorSound != null && audioSource != null)
            {
                audioSource.clip = hardClosingDoorSound;
                audioSource.Play();

                yield return new WaitForSeconds(hardClosingDoorSound.length);

                AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            }
            else if (audioSource != null)
            {
                // Si no había clip pero pedimos fuente, la liberamos igual
                AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            }

            isMoving = false;
            canBeClosed = true;
        }
    }
}