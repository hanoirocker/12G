using System.Collections;
using TwelveG.GameController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.PlayerController
{
    public class ProceduralFaint : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerRootTransform;

        [Header("Procedural Animation Settings")]
        [Tooltip("Duración total de la caída")]
        [SerializeField] private float duration = 2.0f;

        [Header("Rotation Curves (Variaciones relativas)")]
        [Tooltip("Curva de cabeceo (X). Ej: De 0 a 80.")]
        [SerializeField] private AnimationCurve pitchCurve = AnimationCurve.EaseInOut(0, 0, 1, 80);

        [Tooltip("Curva de giro lateral (Z). Ej: De 0 a 90.")]
        [SerializeField] private AnimationCurve rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 90);

        [Tooltip("Curva de giro de cuello (Y). Ej: De 0 a 30.")]
        [SerializeField] private AnimationCurve yawCurve = AnimationCurve.Linear(0, 0, 1, 20);

        [Header("Position Curves")]
        [Tooltip("Controla el progreso de la bajada (0 = Arriba, 1 = Suelo). Útil para hacer que caiga rápido y rebote un poco.")]
        [SerializeField] private AnimationCurve dropProgressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Collision")]
        [SerializeField] private LayerMask groundLayer;

        private bool isFainting = false;

        private void OnEnable()
        {
            if (isFainting) return;
            StartCoroutine(FaintRoutine());
        }

        private IEnumerator FaintRoutine()
        {
            isFainting = true;

            // Bloqueo de jugador
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            // Estados inciales
            Quaternion startRotation = playerRootTransform.rotation;
            Vector3 startPosition = playerRootTransform.position;

            // Detectar el suelo para evitar clipping
            float floorY = startPosition.y;
            RaycastHit hit;
            if (Physics.Raycast(startPosition, Vector3.down, out hit, 3.0f, groundLayer))
            {
                // El objetivo es un poco arriba del suelo para no clipar
                floorY = hit.point.y + 0.2f;
            }

            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / duration;

                // --- ROTACIÓN RELATIVA ---
                // Evaluamos cuánto rotar en cada eje según el tiempo
                float xOffset = pitchCurve.Evaluate(normalizedTime); // Cabeceo
                float yOffset = yawCurve.Evaluate(normalizedTime);   // Giro cuello
                float zOffset = rollCurve.Evaluate(normalizedTime);  // Lado

                // Creamos la rotación del "desmayo"
                Quaternion faintOffsetRotation = Quaternion.Euler(xOffset, yOffset, zOffset);

                // SUMAMOS (Multiplicamos Quaternions) la variación a la rotación original
                // Esto garantiza que si mirabas al techo, caes mirando al techo + el giro de caída.
                playerRootTransform.rotation = startRotation * faintOffsetRotation;


                // --- POSICIÓN ---
                // Evaluamos el progreso de la caída (0 a 1)
                float dropT = dropProgressCurve.Evaluate(normalizedTime);

                // Interpolamos entre la altura inicial y la altura del suelo detectada
                float currentY = Mathf.Lerp(startPosition.y, floorY, dropT);

                // Mantenemos X y Z originales, solo bajamos Y
                playerRootTransform.position = new Vector3(startPosition.x, currentY, startPosition.z);

                yield return null;
            }

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 6f));
        }
    }
}