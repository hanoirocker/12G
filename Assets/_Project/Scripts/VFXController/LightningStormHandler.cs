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
        [Tooltip("Curva para simular el parpadeo del rayo.")]
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

        [Header("Audio Settings - Thunder")]
        [SerializeField, Range(0f, 200f)] private float soundMaxDistance = 200f;
        [SerializeField] private AudioClip thunderSoundClip;
        [SerializeField, Range(0f, 1f)] private float thunderVolume = 1f;
        [Space(5)]
        [SerializeField] private float speedOfSoundDelay = 1f;

        [Header("Audio Settings - Ground Vibration")]
        [SerializeField] private AudioClip groundVibrationClip;
        [SerializeField, Range(0f, 1f)] private float vibrationVolume = 0.8f;
        [SerializeField, Range(0f, 5f)] private float fallbackVibrationDuration = 5f;

        [Header("Camera Shake Settings")]
        [SerializeField, Range(0f, 5f)] private float shakeAmplitude = 1.5f;
        [SerializeField, Range(0f, 10f)] private float shakeFrequency = 2.0f;

        private CinemachineVirtualCamera currentActiveCamera;
        private VirtualCamerasHandler virtualCamerasHandler;

        private bool isFlashingLight = false;

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
            (Transform strikePos, float distFactor) = GetThunderData(isClose: true);

            StartCoroutine(FlashRoutine(distFactor));
            StartCoroutine(ThunderAudioRoutine(distFactor, strikePos, isCloseThunder: true));
            StartCoroutine(GroundVibrationsRouteine());
        }

        private IEnumerator GroundVibrationsRouteine()
        {
            yield return new WaitForSeconds(0.25f);

            if (groundVibrationClip != null)
            {
                // Variables locales: Seguras para ejecución simultánea
                AudioSource vibSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.HouseStereoAmbient);
                AudioSourceState vibState = vibSource.GetSnapshot();

                vibSource.clip = groundVibrationClip;
                vibSource.volume = vibrationVolume;
                vibSource.Play();
                yield return new WaitForSeconds(groundVibrationClip.length);

                AudioUtils.StopAndRestoreAudioSource(vibSource, vibState);
            }

            float vibrationDuration = thunderSoundClip != null ? thunderSoundClip.length * 1.1f : fallbackVibrationDuration;

            StartCoroutine(virtualCamerasHandler.CameraShakeRoutine(shakeAmplitude, shakeFrequency, vibrationDuration));
        }

        private IEnumerator ThunderRoutine()
        {
            while (true)
            {
                (Transform strikePos, float distFactor) = GetThunderData(isClose: false);

                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                StartCoroutine(FlashRoutine(distFactor));
                StartCoroutine(ThunderAudioRoutine(distFactor, strikePos));
            }
        }

        private (Transform, float) GetThunderData(bool isClose)
        {
            Transform targetTransform;

            if (isClose)
            {
                targetTransform = closeSkyPositions[Random.Range(0, closeSkyPositions.Length)];
            }
            else
            {
                targetTransform = skyPositions[Random.Range(0, skyPositions.Length)];
            }

            currentActiveCamera = virtualCamerasHandler?.GetCurrentActiveCamera();
            float distanceFactor = 1f;

            if (currentActiveCamera != null)
            {
                float distanceToPlayer = Vector3.Distance(targetTransform.position, currentActiveCamera.transform.position);
                distanceFactor = Mathf.Clamp01(soundMaxDistance / distanceToPlayer);
            }
            else
            {
                distanceFactor = 0.5f;
            }

            return (targetTransform, distanceFactor);
        }

        private IEnumerator FlashRoutine(float distanceIncidenceFactor)
        {
            isFlashingLight = true;
            float timer = 0f;

            while (timer < flashDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / flashDuration;

                float curveVal = flashProfile.Evaluate(progress);

                if (lightningLight != null)
                {
                    lightningLight.intensity = curveVal * maxIntensity * distanceIncidenceFactor;
                }

                yield return null;
            }

            if (lightningLight != null) lightningLight.intensity = 0f;
            isFlashingLight = false;
        }

        private IEnumerator ThunderAudioRoutine(float currentDistanceFactor, Transform soundOrigin, bool isCloseThunder = false)
        {
            AudioSource localSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Environment);

            if (localSource == null) yield break;

            AudioSourceState localState = localSource.GetSnapshot();

            float delay = isCloseThunder ? 0f : currentDistanceFactor * speedOfSoundDelay;

            yield return new WaitForSeconds(delay);

            if (thunderSoundClip != null)
            {
                localSource.clip = thunderSoundClip;
                localSource.loop = false;
                localSource.volume = thunderVolume;

                if (soundOrigin != null)
                    localSource.transform.position = soundOrigin.position;

                localSource.maxDistance = soundMaxDistance;
                localSource.pitch = Random.Range(0.9f, 1.15f);
                localSource.Play();

                float duration = AudioUtils.CalculateDurationWithPitch(localSource.clip, localSource.pitch);
                yield return new WaitForSeconds(duration);

                AudioUtils.StopAndRestoreAudioSource(localSource, localState);
            }
            else
            {
                AudioUtils.StopAndRestoreAudioSource(localSource, localState);
            }
        }
    }
}