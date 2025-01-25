namespace TwelveG.Utils
{
    using Cinemachine;
    using UnityEngine;
    using TwelveG.PlayerController;

    public class MainCameraHandler : MonoBehaviour
    {
        public void SetCurrentCamera(Component sender, object data)
        {
            if(data != null)
            {
                GetComponent<HeadLookAround>().activeVirtualCamera = (CinemachineVirtualCamera)data;
                GetComponent<CameraZoom>().activeVirtualCamera = (CinemachineVirtualCamera)data;
            }
        }
    }
} 