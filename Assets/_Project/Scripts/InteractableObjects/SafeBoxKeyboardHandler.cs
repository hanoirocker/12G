using System.Collections;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.PlayerController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class SafeBoxKeyboardHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject safeBoxCanvas;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip inputClip;
        [SerializeField] private AudioClip errorClip;
        [SerializeField] private AudioClip unlockClip;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.7f;

        [Header("Safe Combination")]
        [SerializeField] private string correctCombination = "1234";

        [Header("Event SO references")]
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onMainCameraSettings;
        public GameEventSO onPlayerControls;

        private bool playerCanExit = false;
        private bool isProcessing = false; // Bloquea input durante error/desbloqueo
        private string currentInput = "";
        private AudioSource audioSource;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && playerCanExit)
            {
                this.enabled = false;
            }
        }

        void OnEnable()
        {
            audioSource = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
            StartCoroutine(InteractWithKeyboard());
        }

        void OnDisable()
        {
            StartCoroutine(QuitKeyboard());
        }

        private IEnumerator InteractWithKeyboard()
        {
            onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 1));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.SafeBox, true));
            yield return new WaitForSeconds(1f);
            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
            onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));
            onPlayerControls.Raise(this, new EnablePauseMenuCanvasAccess(false));
            safeBoxCanvas.SetActive(true);

            playerCanExit = true;
        }

        private IEnumerator QuitKeyboard()
        {
            playerCanExit = false;

            safeBoxCanvas.SetActive(false);
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.SafeBox, false));
            yield return new WaitForSeconds(1f);
            onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
            onPlayerControls.Raise(this, new EnablePauseMenuCanvasAccess(true));
        }

        public void ProcessNumberInput(int value)
        {
            if (isProcessing) return;

            PlayClip(inputClip);

            currentInput += value.ToString();

            if (currentInput.Length >= 5)
            {
                if (currentInput == correctCombination)
                {
                    StartCoroutine(HandleCorrectCombination());
                }
                else
                {
                    StartCoroutine(HandleIncorrectCombination());
                }
            }
        }

        private IEnumerator HandleIncorrectCombination()
        {
            isProcessing = true;
            yield return new WaitForSeconds(0.5f);
            PlayClip(errorClip);

            yield return new WaitForSeconds(errorClip.length);

            currentInput = "";
            isProcessing = false;
        }

        private IEnumerator HandleCorrectCombination()
        {
            isProcessing = true;
            yield return new WaitForSeconds(0.5f);

            PlayClip(unlockClip);
            yield return new WaitForSeconds(unlockClip.length);

            StartCoroutine(QuitKeyboard());
            GetComponent<SafeBoxHandler>().UnlockSafeBox();
            this.enabled = false;
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}