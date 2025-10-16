namespace TwelveG.PlayerController
{
    using Cinemachine;
    using UnityEngine;

    public class CameraZoom : MonoBehaviour
    {
        public CinemachineVirtualCamera activeVirtualCamera;
        public float currentActiveCameraDefaultFOV;

        [Header("Zoom Settings")]
        public float zoomSpeed = 10f;
        
        private float zoomFOV = 40f;
        private bool isZooming = false;

        private void Update()
        {
            ZoomInCamera();
        }

        private void ZoomInCamera()
        {
            if (Input.GetMouseButton(1))
            {
                activeVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(activeVirtualCamera.m_Lens.FieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
                isZooming = true;

            }
            else
            {
                activeVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(activeVirtualCamera.m_Lens.FieldOfView, currentActiveCameraDefaultFOV, Time.deltaTime * zoomSpeed);
                isZooming = false;
            }
        }

        public bool playerIsZooming()
        {
            return isZooming;
        }
    }

}