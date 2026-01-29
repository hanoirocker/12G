using TwelveG.PlayerController;
using System.Collections;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.AudioController;

namespace TwelveG.InteractableObjects
{
    public class SlideDoorHandler : MonoBehaviour, IInteractable
    {
        [Header("Object Settings")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpen;

        [Tooltip("Dirección y distancia del deslizamiento (Local). Ej: (1.45, 0, 0)")]
        [SerializeField] private Vector3 slideOffset = new Vector3(1.45f, 0f, 0f);

        [Header("Movement Settings")]
        [Tooltip("Curva de movimiento. Recomendado: Ease-Out para puertas pesadas.")]
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField, Range(0.5f, 2.0f)] float fallbackDuration = 1.0f;

        [Header("Audio Settings")]
        [SerializeField] AudioClip openingDoorSound;
        [SerializeField] AudioClip closingDoorSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.8f;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        private bool isMoving;
        private Vector3 closedLocalPosition;
        private Vector3 openLocalPosition;

        private void Awake()
        {
            isMoving = false;

            if (doorIsOpen)
            {
                openLocalPosition = door.transform.localPosition;
                closedLocalPosition = door.transform.localPosition - slideOffset;
            }
            else
            {
                closedLocalPosition = door.transform.localPosition;
                openLocalPosition = door.transform.localPosition + slideOffset;
            }
        }

        private void ToggleDoor()
        {
            Vector3 targetLocalPos = doorIsOpen ? closedLocalPosition : openLocalPosition;
            StartCoroutine(SlideDoorRoutine(targetLocalPos));
        }

        private IEnumerator SlideDoorRoutine(Vector3 targetLocalPos)
        {
            isMoving = true;

            bool isOpeningAction = !doorIsOpen;

            AudioClip clip = isOpeningAction ? openingDoorSound : closingDoorSound;
            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                door.transform,
                clipsVolume
            );

            float duration = fallbackDuration;

            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);

                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    duration = AudioUtils.CalculateDurationWithPitch(clip, audioSource.pitch);
                }
            }

            float elapsedTime = 0f;
            Vector3 startLocalPos = door.transform.localPosition;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                float curveT = movementCurve.Evaluate(t);

                door.transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, curveT);
                yield return null;
            }

            door.transform.localPosition = targetLocalPos;
            doorIsOpen = !doorIsOpen;

            if (audioSource != null)
            {
                AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
                AudioManager.Instance.PoolsHandler.ReleaseAudioSource(audioSource);
            }

            isMoving = false;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDoor();
            return true;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return !isMoving;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_open;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor) => throw new System.NotImplementedException();

        public (ObservationTextSO, float timeUntilShown) GetFallBackText() => throw new System.NotImplementedException();
    }
}