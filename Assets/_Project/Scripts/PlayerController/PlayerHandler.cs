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
        public static PlayerHandler Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject playerCapsule;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private PlayerSpotter playerSpotter;

        [SerializeField] private FPController fpController;

        [SerializeField] private AudioSource playerCapsuleAudioSource;
        [SerializeField] private Transform playerCameraRoot;

        public AudioSource PlayerCapsuleAudioSource => playerCapsuleAudioSource;
        public FPController FPController => fpController;
        public PlayerSpotter PlayerSpotter => playerSpotter;

        private HouseArea currentHouseArea = HouseArea.None;
        private PlayerShortcuts playerShortcuts;

        private PlayerInput playerInput;
        private CharacterController characterController;
        private AudioSource playerAudioSource;
        private HeadBobController headBobController;

        private PlayerInteraction playerInteraction;
        private PlayerContemplation playerContemplation;
        private PlayerAddItem playerAddItem;
        private CameraZoom cameraZoom;
        private HeadLookAround headLookAround;
        private PlayerInventory playerInventory;

        private bool playerIsExaminingObject = false;
        private bool playerCanExamineObjects = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            playerShortcuts = GetComponent<PlayerShortcuts>();

            playerInput = playerCapsule.GetComponent<PlayerInput>();
            characterController = playerCapsule.GetComponent<CharacterController>();
            playerAudioSource = playerCapsule.GetComponent<AudioSource>();
            headBobController = playerCapsule.GetComponent<HeadBobController>();

            playerInteraction = mainCamera.GetComponent<PlayerInteraction>();
            playerContemplation = mainCamera.GetComponent<PlayerContemplation>();
            playerAddItem = mainCamera.GetComponent<PlayerAddItem>();
            cameraZoom = mainCamera.GetComponent<CameraZoom>();
            headLookAround = mainCamera.GetComponent<HeadLookAround>();
            playerInventory = mainCamera.GetComponentInChildren<PlayerInventory>();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        void Start()
        {
            VFXManager.Instance?.RegisterPlayer(playerCameraRoot);
            AudioManager.Instance.PlayerSoundsHandler.RegisterPlayerHandler(this);
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
                    fpController.EnableSprint(cmd.Enabled);
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

        public void PlayerOnDialogMode(bool isInDialog)
        {
            if (isInDialog)
            {
                // Cancelar cualquier examen en curso si existe
                mainCamera.GetComponentInChildren<ExaminableObject>()?.CancelExaminationMode();
            }

            // Alternar la posibilidad de que el jugador examine 
            // o contemple objetos
            playerCanExamineObjects = !isInDialog;
            playerContemplation.enabled = !isInDialog;
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
            playerSpotter.enabled = enable;
        }

        private void HandleExamination(bool isExamining)
        {
            playerIsExaminingObject = isExamining;

            if (playerInventory.PlayerHasFlashlightEquipped())
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
            fpController.enabled = enabled;
            playerAudioSource.enabled = enabled;
            AudioManager.Instance.PlayerSoundsHandler.enabled = enabled;
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

        public bool PlayerIsExaminingObject()
        {
            return playerIsExaminingObject;
        }

        public bool PlayerCanExamineObjects()
        {
            return playerCanExamineObjects;
        }
    }
}