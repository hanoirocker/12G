namespace TwelveG.PlayerController
{
    using UnityEngine;

    public class PlayerHandler : MonoBehaviour
    {
        [SerializeField] private GameObject playerCapsule;
        [SerializeField] private PlayerShortcuts playerShortcuts;
        [SerializeField] private HeadLookAround headLookAround;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private CameraZoom cameraZoom;

        // Si se emite un evento para habilitar cualquier VC distinta de playerVC,
        // Se deben desactivar los componentes de exploración.
        // Componentes de exploracion:
        // - PlayerSoundsHandler
        // - FPController
        // - DUDA: PlayerInteraction? La mayoria de VC's no requieren del PlayerInteraction !!
        // - PlayerContemplación? Depende de la VC que se active --> agregar condicional eventualmente.
        public void PlayerControls(Component sender, object data)
        {
            if((string)data == "EnablePlayerShortcuts")
            {
                playerShortcuts.enabled = true;
            }
            else if((string)data == "DisablePlayerShortcuts")
            {
                playerShortcuts.enabled = false;
            }
            else if((string)data == "EnablePlayerCapsule")
            {
                playerCapsule.SetActive(true);
            }
            else if((string)data == "DisablePlayerCapsule")
            {
                playerCapsule.SetActive(false);
            }
            else if((string)data == "EnableHeadLookAround")
            {
                headLookAround.enabled = true;
            }
            else if((string)data == "DisableHeadLookAround")
            {
                headLookAround.enabled = false;
            }
            else if((string)data == "DisableMainCamera")
            {
                mainCamera.SetActive(false);
            }
            else if((string)data == "EnableMainCamera")
            {
                mainCamera.SetActive(true);
            }
            else if((string)data == "DisableCameraZoom")
            {
                cameraZoom.enabled = false;
            }
            else if((string)data == "EnableCameraZoom")
            {
                cameraZoom.enabled = true;
            }
        }

        public bool PlayerCapsuleEnabled()
        {
            return playerCapsule.activeSelf;
        }

        public void EnablePlayerCapsule()
        {
            playerCapsule.SetActive(true);
        }

        public void DisablePlayerCapsule()
        {
            playerCapsule.SetActive(false);
        }
    }
}