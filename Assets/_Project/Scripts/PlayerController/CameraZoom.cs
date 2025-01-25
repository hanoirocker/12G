namespace TwelveG.PlayerController
{
    using System.Collections;
    using System.Collections.Generic;
    using Cinemachine;
    using UnityEngine;
    using TwelveG.Utils;

    public class CameraZoom : MonoBehaviour
    {
        public CinemachineVirtualCamera activeVirtualCamera;

        [Header("Zoom Settings")]
        [SerializeField, Range(40, 70f)] float defaultFOV = 60f;

        public float zoomSpeed = 10f;
        private float zoomFOV = 30f;
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