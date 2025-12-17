using UnityEngine;

namespace TwelveG.PlayerController
{
    public class HeadBobController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Settings")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField, Range(0.1f, 30f)] private float smoothTransitionSpeed = 10f;

        [Header("Walking")]
        [SerializeField] private float walkFrequency = 14f; // Velocidad del paso
        [SerializeField] private float walkAmount = 0.05f;  // Distancia vertical

        [Header("Running")]
        [SerializeField] private float runFrequency = 18f;
        [SerializeField] private float runAmount = 0.1f;

        private CharacterController characterController;
        private float defaultPosY = 0;
        private float timer = 0;

        private void Start()
        {
            // Guardamos la altura inicial para saber a dónde volver
            defaultPosY = cameraTransform.localPosition.y;

            characterController = GetComponentInParent<CharacterController>();
        }

        private void Update()
        {
            if (!enableHeadBob) return;

            CheckMotion();
            ResetPosition();
        }

        private void CheckMotion()
        {
            float speed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

            if (characterController != null)
            {
                speed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
            }

            if (speed > 0.1f) // Si nos movemos
            {
                bool isRunning = Input.GetKey(KeyCode.LeftShift);

                PlayMotion(
                    isRunning ? runFrequency : walkFrequency,
                    isRunning ? runAmount : walkAmount
                );
            }
        }

        private void PlayMotion(float frequency, float amplitude)
        {
            timer += Time.deltaTime * frequency;

            float newY = defaultPosY + Mathf.Sin(timer) * amplitude;

            // Aplicamos movimiento a la cámara
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                newY,
                cameraTransform.localPosition.z
            );
        }

        private void ResetPosition()
        {
            // Si el jugador se detiene, la cámara debe volver suavemente a la posición original
            float speed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
            if (characterController != null) speed = characterController.velocity.magnitude;

            if (speed <= 0.1f)
            {
                timer = 0;
                Vector3 newPos = new Vector3(
                    cameraTransform.localPosition.x,
                    Mathf.Lerp(cameraTransform.localPosition.y, defaultPosY, Time.deltaTime * smoothTransitionSpeed),
                    cameraTransform.localPosition.z
                );
                cameraTransform.localPosition = newPos;
            }
        }

        // public void ToggleHeadBob(bool state)
        // {
        //     enableHeadBob = state;
        // }
    }
}