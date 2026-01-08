using System.Collections;
using UnityEngine;
using TwelveG.AudioController;
using TwelveG.Utils;
using Cinemachine;

namespace TwelveG.EnvironmentController
{
    public class LightningStormHandler : MonoBehaviour
    {
        [Header("Lighting Settings")]
        [Tooltip("La luz direccional dedicada exclusivamente al rayo")]
        [SerializeField] private Light lightningLight;
        [SerializeField] private float maxIntensity = 2.0f;
        [Tooltip("Curva para simular el parpadeo del rayo. Configurar como picos rápidos.")]
        [SerializeField]
        private AnimationCurve flashProfile = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.2f, 1),
            new Keyframe(0.3f, 0.2f),
            new Keyframe(0.5f, 0.8f),
            new Keyframe(1, 0)
        );
        [SerializeField] private float flashDuration = 0.5f;

        [Header("Timing Settings")]
        [SerializeField] private float minInterval = 25f;
        [SerializeField] private float maxInterval = 60f;

        [Header("Visual Bolt (Optional)")]
        [SerializeField] private Transform[] skyPositions;
        [SerializeField] private Transform[] closeSkyPositions;

        [Header("Audio Settings")]
        [SerializeField, Range(0f, 200f)] private float soundMaxDistance = 200f;
        [SerializeField] private AudioClip thunderSoundClip;
        [SerializeField, Range(0f, 1f)] private float thunderVolume = 1f;
        [SerializeField] private float speedOfSoundDelay = 1.5f;

        private CinemachineVirtualCamera currentActiveCamera;
        private VirtualCamerasHandler virtualCamerasHandler;
        private AudioSource thunderSource;
        private AudioSourceState audioSourceState;
        private Transform currentStrikeTransform;
        private float currentDistanceFactor = 1f;

        private void Awake()
        {
            virtualCamerasHandler = FindObjectOfType<VirtualCamerasHandler>();
        }

        private void Start()
        {
            if (lightningLight != null) lightningLight.intensity = 0f;
        }

        public void StartConstantThunder()
        {
            StartCoroutine(ThunderRoutine());
        }

        public void StartCloseThunder()
        {
            StopAllCoroutines();
            SetUpThunder(true);
            StartCoroutine(FlashRoutine(1f));
            StartCoroutine(ThunderAudioRoutine(1f));
        }

        private IEnumerator ThunderRoutine()
        {
            while (true)
            {
                // 1. Determinar valores de estado inicial
                SetUpThunder();

                // 2. Esperar tiempo aleatorio para el próximo rayo
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                // 3. Ejecutar el Flash Visual (Luz)
                yield return StartCoroutine(FlashRoutine(currentDistanceFactor));

                // 4. Ejecutar el Sonido (con delay basado en distancia)
                yield return StartCoroutine(ThunderAudioRoutine(currentDistanceFactor));
            }
        }

        private void SetUpThunder(bool isClose = false)
        {
            // Determinar qué tan "lejos" cayó (simulado)
            // 0 = muy cerca (trueno inmediato), 1 = lejos (trueno con delay)
            if(isClose)
            {
                currentStrikeTransform = closeSkyPositions[Random.Range(0, closeSkyPositions.Length)];
            }
            else
            {
                currentStrikeTransform = skyPositions[Random.Range(0, skyPositions.Length)];
            }

            currentActiveCamera = virtualCamerasHandler?.GetCurrentActiveCamera();
            float distanceToPlayer = Vector3.Distance(currentStrikeTransform.position, currentActiveCamera.transform.position);
            Debug.Log("Distance to player: " + distanceToPlayer);
            currentDistanceFactor = Mathf.Clamp01(soundMaxDistance / distanceToPlayer);
            Debug.Log("Distance factor: " + currentDistanceFactor);
        }

        private IEnumerator FlashRoutine(float currentDistanceFactor)
        {
            float timer = 0f;

            while (timer < flashDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / flashDuration;

                // Evaluamos la curva para obtener esa "vibración" de luz
                float curveVal = flashProfile.Evaluate(progress);

                // Aplicamos intensidad
                if (lightningLight != null)
                {
                    lightningLight.intensity = curveVal * maxIntensity * currentDistanceFactor;
                }

                yield return null;
            }

            // Asegurar apagado
            if (lightningLight != null) lightningLight.intensity = 0f;
        }

        private IEnumerator ThunderAudioRoutine(float currentDistanceFactor)
        {
            thunderSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Environment);
            audioSourceState = thunderSource.GetSnapshot();

            // Calculamos el delay: Si está cerca, delay casi 0. Si está lejos, delay alto.
            float delay = currentDistanceFactor * speedOfSoundDelay; // Ej: 1.0 * 3 segundos = 3s delay

            yield return new WaitForSeconds(delay);

            if (thunderSoundClip != null && thunderSource != null)
            {
                // Tip de realismo: Si está lejos, suena más despacio y grave (low pass filter sería ideal, pero volumen sirve)
                thunderSource.clip = thunderSoundClip;
                thunderSource.loop = false;
                thunderSource.volume = thunderVolume;
                thunderSource.transform.position = currentStrikeTransform.position;
                thunderSource.maxDistance = soundMaxDistance;
                thunderSource.pitch = Random.Range(0.9f, 1.15f);
                thunderSource.Play();

                float duration = AudioUtils.CalculateDurationWithPitch(thunderSource.clip, thunderSource.pitch);
                yield return new WaitForSeconds(duration);

                AudioUtils.StopAndRestoreAudioSource(thunderSource, audioSourceState);
                thunderSource = null;
            }
        }
    }
}