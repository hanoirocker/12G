namespace TwelveG.PlayerController
{
    using Cinemachine;
    using UnityEngine;

    public class CameraZoom : MonoBehaviour
    {
        public CinemachineVirtualCamera activeVirtualCamera;

        [Header("Zoom Settings")]
        // TODO: actualmente el default FOV se define acá pero deberiamos
        // pasar este valor a una config general en algun manager .. para que se pueda
        // guardar dicho valor desde los settings y este handler pueda acceder a dicho valor
        [SerializeField, Range(40, 110f)] float defaultFOV = 80f;

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
                activeVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(activeVirtualCamera.m_Lens.FieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
                isZooming = false;
            }
        }

        public bool playerIsZooming()
        {
            return isZooming;
        }
    }

}