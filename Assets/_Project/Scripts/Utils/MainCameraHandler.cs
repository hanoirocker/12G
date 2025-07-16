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

        public void CameraSettings(Component sender, object data)
        {
            CinemachineBrain cinemachineBrain = GetComponent<CinemachineBrain>();
            switch (data)
            {
                case SetCameraBlend cmd:
                    cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(cmd.Style, cmd.Time);
                    break;
                case SetLinealCameraBlend cmd:
                    cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Linear, cmd.Time);
                    break;
                case SetCustomCameraBlend cmd:
                    cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Custom, cmd.Time);
                    break;
                default:
                    Debug.LogWarning($"[MainCameraHandler] Received unknown command: {data}");
                    break;
            }
            // if ((string)data == "EasyInOut2")
            // {
            //     CinemachineBrain cinemachineBrain = GetComponent<CinemachineBrain>();
            //     cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 2f);
            // }
            // else if ((string)data == "Cut")
            // {
            //     CinemachineBrain cinemachineBrain = GetComponent<CinemachineBrain>();
            //     cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
            // }
        }
    }
}