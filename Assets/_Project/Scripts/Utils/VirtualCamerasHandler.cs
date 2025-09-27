namespace TwelveG.Utils
{
    using Cinemachine;
    using UnityEngine;

    public class VirtualCamerasHandler : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera playerVC;
        [SerializeField] private CinemachineVirtualCamera wakeUpVC;
        [SerializeField] private CinemachineVirtualCamera kitchenDeskVC;
        [SerializeField] private CinemachineVirtualCamera pCVC;
        [SerializeField] private CinemachineVirtualCamera backpackVC;
        [SerializeField] private CinemachineVirtualCamera phoneVC;
        [SerializeField] private CinemachineVirtualCamera bedVC;
        [SerializeField] private CinemachineVirtualCamera tvVC;
        [SerializeField] private CinemachineVirtualCamera safeBox;

        [SerializeField] private CinemachineVirtualCamera SofaVC;

        [Header("EventsSO references")]
        public GameEventSO setCurrentCamera;
        public GameEventSO returnCurrentCamera;

        private CinemachineVirtualCamera currentActiveCamera;

        private void Start()
        {
            GetCurrentActiveCamera();
            setCurrentCamera.Raise(this, currentActiveCamera);
        }

        private void GetCurrentActiveCamera()
        {
            CinemachineVirtualCamera[] listOfVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (CinemachineVirtualCamera vc in listOfVirtualCameras)
            {
                if (vc.isActiveAndEnabled)
                {
                    currentActiveCamera = vc;
                }
            }
            // print("From VirtualCamerasHandler, active VC: " + currentActiveCamera.gameObject.name);
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
            else
            {
                Debug.LogWarning($"[VirtualCamerasHandler] Comando desconocido recibido: {data}");
            }

            setCurrentCamera.Raise(this, currentActiveCamera);
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
                VirtualCameraTarget.Sofa => SofaVC,
                VirtualCameraTarget.SafeBox => safeBox,
                _ => null
            };
        }
    }
}