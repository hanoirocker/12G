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
            switch (data)
            {
                case TogglePlayerShortcuts cmd:
                    playerShortcuts.enabled = cmd.Enabled;
                    break;

                case TogglePlayerHeadLookAround cmd:
                    headLookAround.enabled = cmd.Enabled;
                    break;

                case TogglePlayerCapsule cmd:
                    playerCapsule.SetActive(cmd.Enabled);
                    break;

                case TogglePlayerMainCamera cmd:
                    mainCamera.SetActive(cmd.Enabled);
                    break;

                case TogglePlayerCameraZoom cmd:
                    cameraZoom.enabled = cmd.Enabled;
                    break;

                default:
                    Debug.LogWarning($"[PlayerHandler] Comando desconocido recibido: {data}");
                    break;
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