using UnityEngine;

namespace TwelveG.PlayerController
{
    public class DizzinessHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Settings")]
        [Tooltip("Velocity of the sway effect oscillation.")]
        [SerializeField] private float swaySpeed = 1.0f;
        // [Tooltip("Maximum sway angle on the X axis.")]
        // [SerializeField] private float swayAngleX = 12f;
        [Tooltip("Maximum sway angle on the Z axis.")]
        [SerializeField] private float swayAngleZ = 12f;

        private float currentSwaySpeed = 0f;
        private float currentSwayAngleX = 0f;
        private float currentSwayAngleZ = 0f;

        private float timer = 0f;
        private float storedSensitivity;
        private float sensitivityFactor = 1f;

        private Quaternion initialRotation;
        private FPController FPController;

        private void Awake()
        {
            FPController = GetComponent<FPController>();
            if (FPController == null)
            {
                Debug.LogError("[DizzinessHandler]: No se encontró CharacterController en el objeto padre.");
                this.enabled = false;
                return;
            }

            if (cameraTransform == null)
                cameraTransform = GetComponentInChildren<Transform>();

        }

        private void OnEnable()
        {
            // Reducción de la sensibilidad del ratón
            storedSensitivity = FPController.RotationSpeed;
        }

        private void OnDisable()
        {
            FPController.RotationSpeed = storedSensitivity;
        }

        private void Update()
        {
            Quaternion targetRotation = initialRotation;

            timer += Time.deltaTime * currentSwaySpeed;

            // Cálculo del balanceo (Seno y Coseno desincronizados)
            // float rotX = Mathf.Sin(timer) * currentSwayAngleX;
            float rotZ = Mathf.Cos(timer * 0.9f) * currentSwayAngleZ; // 0.9f para desincronizar ejes

            Quaternion dizzinessRotation = Quaternion.Euler(0f, 0f, rotZ);

            initialRotation = cameraTransform.localRotation;
            targetRotation = initialRotation * dizzinessRotation;

            // Aplicamos la rotación suavemente (Slerp)
            // Esto maneja tanto la oscilación del mareo como la recuperación suave al estado normal
            float smoothFactor = currentSwaySpeed;

            FPController.RotationSpeed = storedSensitivity * sensitivityFactor;

            cameraTransform.localRotation = Quaternion.Slerp(
                cameraTransform.localRotation,
                targetRotation,
                Time.deltaTime * smoothFactor
            );
        }

        public void SetDizzinessIntensity(float intensity)
        {
            currentSwaySpeed = swaySpeed * intensity;
            // currentSwayAngleX = swayAngleX * intensity;
            currentSwayAngleZ = swayAngleZ * intensity;

            if (intensity <= 0.55f)
            {
                sensitivityFactor = (1 - intensity);
            }
            else
            {
                sensitivityFactor = 0.75f;
            }
        }
    }
}