namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using Cinemachine;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.UIController;
    using TwelveG.Utils;
    using UnityEngine;

    public class SafeBoxKeyboardHandler : MonoBehaviour
    {
        [Header("Event SO references")]
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onMainCameraSettings;
        public GameEventSO onPlayerControls;

        [Header("Text event SO")]
        [SerializeField] private EventsControlCanvasInteractionTextSO eventsControlCanvasInteractionTextSO_safebox;

        private bool playerCanInputValues = false;
        private bool playerCanExit = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && playerCanExit)
            {
                this.enabled = false;
            }
        }

        void OnEnable()
        {
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
            onPlayerControls.Raise(this, new EnableCanvasControlsAccess(false));

            playerCanInputValues = true;
            playerCanExit = true;
        }

        private IEnumerator QuitKeyboard()
        {
            playerCanInputValues = false;
            playerCanExit = false;

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.SafeBox, false));
            yield return new WaitForSeconds(1f);
            onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            onPlayerControls.Raise(this, new EnableCanvasControlsAccess(true));
        }
    }
}