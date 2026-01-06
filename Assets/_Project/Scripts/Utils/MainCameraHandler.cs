namespace TwelveG.Utils
{
    using Cinemachine;
    using UnityEngine;
    using TwelveG.PlayerController;

    public class MainCameraHandler : MonoBehaviour
    {
        [Header("References")]
        public GameObject lastEventSender = null;

        private GameObject objectUnderExamination = null;
        private CinemachineBrain cinemachineBrain;

        private void Awake()
        {
            cinemachineBrain = GetComponent<CinemachineBrain>();
        }

        public void SetCurrentCamera(Component sender, object data)
        {
            if (data != null)
            {
                CinemachineVirtualCamera cinemachineVirtualCamera = (CinemachineVirtualCamera)data;

                GetComponent<HeadLookAround>().activeVirtualCamera = cinemachineVirtualCamera;
                GetComponent<CameraZoom>().activeVirtualCamera = cinemachineVirtualCamera;
                GetComponent<CameraZoom>().currentActiveCameraDefaultFOV = cinemachineVirtualCamera.m_Lens.FieldOfView;
            }
        }

        public void CameraSettings(Component sender, object data)
        {
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
                case ResetCinemachineBrain cmd:
                    cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
                    break;
                default:
                    Debug.LogWarning($"[MainCameraHandler] Received unknown command: {data}");
                    break;
            }
        }

        public void InstatiateExaminableObject(Component sender, object data)
        {
            if (data != null)
            {
                lastEventSender = sender.gameObject;
                objectUnderExamination = Instantiate((GameObject)data, gameObject.transform);
            }
        }

        public void RemoveExaminationObject()
        {
            Destroy(objectUnderExamination);
            objectUnderExamination = null;
        }
    }
}