using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.VFXController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwelveG.PlayerController
{
    public class PlayerHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject playerCapsule;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private Transform playerCameraRoot;

        [Header("Player Settings")]

        private HouseArea currentHouseArea = HouseArea.None;
        private PlayerShortcuts playerShortcuts;

        private PlayerInput playerInput;
        private CharacterController characterController;
        private AudioSource playerAudioSource;
        private FPController fPController;
        private PlayerSoundsHandler playerSoundsHandler;
        private HeadBobController headBobController;

        private PlayerInteraction playerInteraction;
        private PlayerContemplation playerContemplation;
        private PlayerAddItem playerAddItem;
        private CameraZoom cameraZoom;
        private HeadLookAround headLookAround;
        private PlayerInventory playerInventory;

        private void Awake()
        {
            playerShortcuts = GetComponent<PlayerShortcuts>();

            playerInput = playerCapsule.GetComponent<PlayerInput>();
            characterController = playerCapsule.GetComponent<CharacterController>();
            playerAudioSource = playerCapsule.GetComponent<AudioSource>();
            fPController = playerCapsule.GetComponent<FPController>();
            playerSoundsHandler = playerCapsule.GetComponent<PlayerSoundsHandler>();
            headBobController = playerCapsule.GetComponent<HeadBobController>();

            playerInteraction = mainCamera.GetComponent<PlayerInteraction>();
            playerContemplation = mainCamera.GetComponent<PlayerContemplation>();
            playerAddItem = mainCamera.GetComponent<PlayerAddItem>();
            cameraZoom = mainCamera.GetComponent<CameraZoom>();
            headLookAround = mainCamera.GetComponent<HeadLookAround>();
            playerInventory = mainCamera.GetComponentInChildren<PlayerInventory>();
        }

        void Start()
        {
            VFXManager.Instance?.RegisterPlayer(playerCameraRoot);
        }

        public void PlayerControls(Component sender, object data)
        {
            switch (data)
            {
                case ToggleToObjectExamination cmd:
                    bool isActive = !cmd.Enabled;

                    ToggleInteractionModules(isActive);
                    SwitchPlayerControllers(isActive);

                    playerShortcuts.playerCanOpenPauseMenu = isActive;
                    HandleExamination(cmd.Enabled);
                    break;

                case EnableControlCanvasAccess cmd:
                    playerShortcuts.playerCanOpenCanvasControls = cmd.Enabled;
                    break;

                case EnablePauseMenuCanvasAccess cmd:
                    playerShortcuts.playerCanOpenPauseMenu = cmd.Enabled;
                    break;

                case EnablePlayerSprint cmd:
                    fPController.EnableSprint(cmd.Enabled);
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

        public void PlayerDyingMode()
        {
            // Cancelar cualquier examen en curso si existe
            mainCamera.GetComponentInChildren<ExaminableObject>()?.CancelExaminationMode();

            // Desactivar accesos directos
            playerShortcuts.enabled = false;
            playerShortcuts.playerCanOpenPauseMenu = false;

            // Apagar todos los sistemas de control e interacción
            SwitchPlayerControllers(false);
            ToggleInteractionModules(false);
        }

        public void PauseGame(Component sender, object data)
        {
            bool enable = !(bool)data;

            playerCapsule.SetActive(enable);
            playerContemplation.enabled = enable;
            playerInteraction.enabled = enable;
        }

        private void HandleExamination(bool isExamining)
        {
            if (playerInventory.PlayerIsUsingFlashlight())
            {
                var camLight = mainCamera.GetComponent<Light>();
                if (camLight != null) camLight.enabled = isExamining;
            }

            playerInventory.HandleExaminationWhileUsingItems(isExamining);
        }

        // Agrupa todos los scripts de interacción con el entorno
        private void ToggleInteractionModules(bool enabled)
        {
            playerInteraction.enabled = enabled;
            playerContemplation.enabled = enabled;
            playerAddItem.enabled = enabled;
            cameraZoom.enabled = enabled;
        }

        // Agrupa todos los scripts de movimiento y input físico
        private void SwitchPlayerControllers(bool enabled)
        {
            fPController.enabled = enabled;
            playerAudioSource.enabled = enabled;
            playerSoundsHandler.enabled = enabled;
            playerInput.enabled = enabled;
            characterController.enabled = enabled;
            headBobController.enabled = enabled;
        }

        public void OnPlayerEnteredHouseArea(Component sender, object data)
        {
            HouseArea houseArea = (HouseArea)data;
            currentHouseArea = houseArea;
        }

        public HouseArea GetCurrentHouseArea()
        {
            return currentHouseArea;
        }
    }
}