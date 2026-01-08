using Cinemachine;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.Utils
{
    public class VirtualCamerasHandler : MonoBehaviour
    {
        [Header("VC's references")]
        [SerializeField] private CinemachineVirtualCamera playerVC;
        [SerializeField] private CinemachineVirtualCamera wakeUpVC;
        [SerializeField] private CinemachineVirtualCamera kitchenDeskVC;
        [SerializeField] private CinemachineVirtualCamera pCVC;
        [SerializeField] private CinemachineVirtualCamera backpackVC;
        [SerializeField] private CinemachineVirtualCamera phoneVC;
        [SerializeField] private CinemachineVirtualCamera bedVC;
        [SerializeField] private CinemachineVirtualCamera tvVC;
        [SerializeField] private CinemachineVirtualCamera safeBox;
        [SerializeField] private CinemachineVirtualCamera sofaVC;
        [SerializeField] private CinemachineVirtualCamera flashlightVC;
        [SerializeField] private CinemachineVirtualCamera focusVC;


        [Header("EventsSO references")]
        public GameEventSO setCurrentCamera;
        public GameEventSO returnCurrentCamera;

        private GameObject playerCameraRoot;
        private Transform playerCapsuleTransform;
        private CinemachineVirtualCamera currentActiveCamera;
        private CinemachineVirtualCamera lastActiveCamera;

        private void Start()
        {
            playerCameraRoot = GameObject.FindWithTag("CinemachineTarget");
            playerCapsuleTransform = GameObject.FindGameObjectWithTag("PlayerCapsule")
                .GetComponent<Transform>();

            if (playerCameraRoot == null)
            {
                Debug.LogError("No se encontró el GameObject con el tag 'CinemachineTarget'");
            }

            playerVC.m_Follow = playerCameraRoot.transform;
            focusVC.m_Follow = playerCameraRoot.transform;

            SetCurrentActiveCamera();
            setCurrentCamera.Raise(this, currentActiveCamera);
        }

        public CinemachineVirtualCamera GetCurrentActiveCamera()
        {
            return currentActiveCamera;
        }

        private void SetCurrentActiveCamera()
        {
            CinemachineVirtualCamera[] listOfVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (CinemachineVirtualCamera vc in listOfVirtualCameras)
            {
                if (vc.isActiveAndEnabled)
                {
                    currentActiveCamera = vc;
                }
            }
        }

        // TODO: ver la forma de simplificar el sistema, actualmente si se desactiva o activa una cam != playerVC, 
        // se asume que se debe modificar playerVC. Y si en algun momento queremos pasar de una cámara a otra que no sea playerVC?
        public void VirtualCamerasControls(Component sender, object data)
        {
            if (data is ToggleVirtualCamera cmd)
            {
                CinemachineVirtualCamera targetVC = GetVCByEnum(cmd.Target);

                targetVC.enabled = cmd.Enabled;

                if (cmd.Target != VirtualCameraTarget.Player)
                    playerVC.enabled = !cmd.Enabled;

                currentActiveCamera = cmd.Enabled ? targetVC : playerVC;

                if (cmd.Target == VirtualCameraTarget.Backpack && cmd.Enabled)
                {
                    ReturnVCInstance();
                }

                setCurrentCamera.Raise(this, currentActiveCamera);
            }
            else if (data is ToggleIntoCinematicCameras enable)
            {
                if (enable.Enabled)
                {
                    lastActiveCamera = currentActiveCamera;
                    currentActiveCamera.enabled = false;
                }
                else
                {
                    lastActiveCamera.enabled = true;
                    lastActiveCamera = null;
                }
            }
            else if (data is LookAtTarget lookAtCmd)
            {
                HandleFocusCamera(lookAtCmd.LookAtTransform);
            }
            else
            {
                Debug.LogWarning($"[VirtualCamerasHandler] Comando desconocido recibido: {data}");
            }

            setCurrentCamera.Raise(this, currentActiveCamera);
        }

        private void HandleFocusCamera(Transform target)
        {
            if (target != null)
            {
                focusVC.m_LookAt = target;

                // Para evitar que la cámara viaje desde el origen del mundo,
                // forzamos su posición a la de la cámara actual antes de encenderla.
                focusVC.transform.position = currentActiveCamera.transform.position;
                focusVC.transform.rotation = currentActiveCamera.transform.rotation;

                focusVC.enabled = true;
                focusVC.Priority = 100;
            }
            else
            {
                playerCapsuleTransform.position = focusVC.transform.position;
                Vector3 focusEuler = focusVC.transform.rotation.eulerAngles;
                playerCapsuleTransform.rotation = Quaternion.Euler(0, focusEuler.y, 0);

                playerCameraRoot.transform.localRotation = Quaternion.Euler(focusEuler.x, 0, 0);

                focusVC.Priority = 0;
                focusVC.enabled = false;
                focusVC.m_LookAt = null;

                currentActiveCamera = playerVC;

                FPController fpController = playerCameraRoot.GetComponentInParent<FPController>();

                if (fpController != null)
                {
                    fpController.SyncRotationWithCurrentCamera();
                }
            }
        }

        private void ReturnVCInstance()
        {
            returnCurrentCamera.Raise(this, currentActiveCamera);
        }

        private CinemachineVirtualCamera GetVCByEnum(VirtualCameraTarget target)
        {
            return target switch
            {
                VirtualCameraTarget.Player => playerVC,
                VirtualCameraTarget.WakeUp => wakeUpVC,
                VirtualCameraTarget.KitchenDesk => kitchenDeskVC,
                VirtualCameraTarget.Bed => bedVC,
                VirtualCameraTarget.PC => pCVC,
                VirtualCameraTarget.Backpack => backpackVC,
                VirtualCameraTarget.Phone => phoneVC,
                VirtualCameraTarget.TV => tvVC,
                VirtualCameraTarget.Sofa => sofaVC,
                VirtualCameraTarget.SafeBox => safeBox,
                VirtualCameraTarget.Flashlight => flashlightVC,
                VirtualCameraTarget.Focus => focusVC,
                _ => null
            };
        }
    }
}