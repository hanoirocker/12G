namespace TwelveG.PlayerController
{
    using TwelveG.AudioController;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerHandler : MonoBehaviour
    {
        [Header("Player Capsule References")]
        [SerializeField] private GameObject playerCapsule;
        [SerializeField] private CapsuleCollider playerCapsuleCollider;
        [SerializeField] private FPController fPController;
        [SerializeField] private AudioSource playerAudioSource;
        [SerializeField] private PlayerSoundsHandler playerSoundsHandler;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private CharacterController characterController;

        [Header("Player Camera References")]
        [SerializeField] private HeadLookAround headLookAround;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private CameraZoom cameraZoom;

        private PlayerShortcuts playerShortcuts;

        private void Awake()
        {
            playerShortcuts = GetComponent<PlayerShortcuts>();
        }

        public void PlayerControls(Component sender, object data)
        {
            switch (data)
            {
                case EnableControlCanvasAccess cmd:
                    playerShortcuts.playerCanOpenCanvasControls = cmd.Enabled;
                    break;
                case EnablePauseMenuCanvasAccess cmd:
                    playerShortcuts.playerCanOpenPauseMenu = cmd.Enabled;
                    break;
                case TogglePlayerCapsule cmd:
                    playerCapsule.SetActive(cmd.Enabled);
                    break;
                case TogglePlayerMainCamera cmd:
                    mainCamera.SetActive(cmd.Enabled);
                    break;
                case EnablePlayerShortcuts cmd:
                    playerShortcuts.enabled = cmd.Enabled;
                    break;
                case EnablePlayerHeadLookAround cmd:
                    headLookAround.enabled = cmd.Enabled;
                    break;
                case EnablePlayerControllers cmd:
                    SwitchPlayerControllers(cmd.Enabled);
                    break;
                case EnablePlayerCameraZoom cmd:
                    cameraZoom.enabled = cmd.Enabled;
                    break;

                default:
                    Debug.LogWarning($"[PlayerHandler] Comando desconocido recibido: {data}");
                    break;
            }
        }

        private void SwitchPlayerControllers(bool enabled)
        {
            playerCapsuleCollider.enabled = enabled;
            fPController.enabled = enabled;
            playerAudioSource.enabled = enabled;
            playerSoundsHandler.enabled = enabled;
            playerInput.enabled = enabled;
            characterController.enabled = enabled;
        }

        public void PauseGame(Component sender, object data)
        {
            TogglePlayerCapsule(!(bool)data);
        }

        private void TogglePlayerCapsule(bool activate)
        {
            playerCapsule.SetActive(activate);
        }
    }
}