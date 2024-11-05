namespace TwelveG.PlayerController
{
    using UnityEngine;
    using Cinemachine;
    using TwelveG.Utils;

    public class HeadLookAround : MonoBehaviour
    {
        public CinemachineVirtualCamera activeVirtualCamera;

        [Header("Mouse Settings")]
        public float mouseSensitivity = 100f;

        [Header("Rotation Limits")]
        public float maxXRotation = 30f;
        public float maxYRotation = 30f;

        private float xRotation = 0f;
        private float yRotation = 0f;

        private Quaternion initialRotation;

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            initialRotation = transform.localRotation;

            if (activeVirtualCamera != null)
            {
                initialRotation = activeVirtualCamera.transform.localRotation;
            }
        }

        private void Update()
        {
            MoveCameraAround();
        }

        private void MoveCameraAround()
        {
            // Get the mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Calculate new rotation values
            yRotation += mouseX;
            xRotation -= mouseY;

            // Clamp the rotation values to the specified limits
            xRotation = Mathf.Clamp(xRotation, -maxXRotation, maxXRotation);
            yRotation = Mathf.Clamp(yRotation, -maxYRotation, maxYRotation);

            // Apply the rotation to the active virtual camera
            if (activeVirtualCamera != null)
            {
                Quaternion xQuat = Quaternion.AngleAxis(xRotation, Vector3.right);
                Quaternion yQuat = Quaternion.AngleAxis(yRotation, Vector3.up);
                activeVirtualCamera.transform.localRotation = initialRotation * Quaternion.Euler(xRotation, yRotation, 0f);
            }
        }
    }
}
