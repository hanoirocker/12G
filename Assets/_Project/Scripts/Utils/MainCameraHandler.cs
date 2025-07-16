namespace TwelveG.Utils
{
    using Cinemachine;
    using UnityEngine;
    using TwelveG.PlayerController;

    public class MainCameraHandler : MonoBehaviour
    {
        public void SetCurrentCamera(Component sender, object data)
        {
            if (data != null)
            {
                GetComponent<HeadLookAround>().activeVirtualCamera = (CinemachineVirtualCamera)data;
                GetComponent<CameraZoom>().activeVirtualCamera = (CinemachineVirtualCamera)data;
            }
        }

        // TODO: implement new eventSO structure
        public void CameraSettings(Component sender, object data)
        {
            if ((string)data == "EasyInOut2")
            {
                CinemachineBrain cinemachineBrain = GetComponent<CinemachineBrain>();
                cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 2f);
            }
            else if ((string)data == "Cut")
            {
                CinemachineBrain cinemachineBrain = GetComponent<CinemachineBrain>();
                cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
            }
        }
    }
}