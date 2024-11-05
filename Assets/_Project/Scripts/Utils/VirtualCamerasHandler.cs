namespace TwelveG.Utils
{
    using System;
    using System.Collections.Generic;
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
            List<CinemachineVirtualCamera> virtualCameras = new List<CinemachineVirtualCamera>();
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
            if ((string)data == "EnablePlayerVC")
            {
                playerVC.enabled = true;
                currentActiveCamera = playerVC;
            }
            else if ((string)data == "DisablePlayerVC")
            {
                playerVC.enabled = false;
            }
            else if ((string)data == "EnableWakeUpVC")
            {
                wakeUpVC.enabled = true;
                playerVC.enabled = false;
                currentActiveCamera = wakeUpVC;
            }
            else if ((string)data == "DisableWakeUpVC")
            {
                playerVC.enabled = true;
                wakeUpVC.enabled = false;
                currentActiveCamera = playerVC;
            }
            else if ((string)data == "EnableKitchenDeskVC")
            {
                kitchenDeskVC.enabled = true;
                playerVC.enabled = false;
                currentActiveCamera = kitchenDeskVC;
            }
            else if ((string)data == "DisableKitchenDeskVC")
            {
                playerVC.enabled = true;
                kitchenDeskVC.enabled = false;
                currentActiveCamera = playerVC;
            }
            else if ((string)data == "EnablePCVC")
            {
                pCVC.enabled = true;
                playerVC.enabled = false;
                currentActiveCamera = pCVC;
            }
            else if ((string)data == "DisablePCVC")
            {
                playerVC.enabled = true;
                pCVC.enabled = false;
                currentActiveCamera = playerVC;
            }
            else if ((string)data == "EnableBedVC")
            {
                bedVC.enabled = true;
                playerVC.enabled = false;
                currentActiveCamera = bedVC;
            }
            else if ((string)data == "DisableBedVC")
            {
                playerVC.enabled = true;
                bedVC.enabled = false;
                currentActiveCamera = playerVC;
            }
            else if ((string)data == "EnableBackpackVC")
            {
                backpackVC.enabled = true;
                playerVC.enabled = false;
                currentActiveCamera = backpackVC;

                ReturnVCInstance();
            }
            else if ((string)data == "DisableBackpackVC")
            {
                playerVC.enabled = true;
                backpackVC.enabled = false;
                currentActiveCamera = playerVC;
            }
            else if ((string)data == "EnablePhoneVC")
            {
                phoneVC.enabled = true;
                playerVC.enabled = false;
                currentActiveCamera = phoneVC;
            }
            else if ((string)data == "DisablePhoneVC")
            {
                playerVC.enabled = true;
                phoneVC.enabled = false;
                currentActiveCamera = playerVC;
            }

            setCurrentCamera.Raise(this, currentActiveCamera);
        }

        private void ReturnVCInstance()
        {
            returnCurrentCamera.Raise(this, currentActiveCamera);
        }
    }
}