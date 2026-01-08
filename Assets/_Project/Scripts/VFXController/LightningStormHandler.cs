using System.Collections;
using UnityEngine;
using TwelveG.AudioController;

namespace TwelveG.EnvironmentController
{
    public class LightningStormHandler : MonoBehaviour
    {
        [Header("Lighting Settings")]
        [Tooltip("La luz direccional dedicada exclusivamente al rayo (debe empezar apagada o en intensidad 0)")]
        [SerializeField] private Light lightningLight;
        [SerializeField] private float maxIntensity = 2.0f;
        [Tooltip("Curva para simular el parpadeo del rayo. Configurar como picos rápidos.")]
        [SerializeField] private AnimationCurve flashProfile = new AnimationCurve(
            new Keyframe(0, 0), 
            new Keyframe(0.2f, 1), 
            new Keyframe(0.3f, 0.2f), 
            new Keyframe(0.5f, 0.8f), 
            new Keyframe(1, 0)
        );
        [SerializeField] private float flashDuration = 0.5f;

        [Header("Timing Settings")]
        [SerializeField] private float minInterval = 5f;
        [SerializeField] private float maxInterval = 15f;

        [Header("Visual Bolt (Optional)")]
        [SerializeField] private GameObject lightningBoltObject;
        [SerializeField] private Transform[] skyPositions;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip thunderSoundClip;
        [SerializeField] private float speedOfSoundDelay = 1.5f;
        private AudioSource thunderSource;
        private AudioSourceState audioSourceState;

        private void Start()
        {
            if (lightningLight != null) lightningLight.intensity = 0f;
            if (lightningBoltObject != null) lightningBoltObject.SetActive(false);

            StartThunder();
        }
        public void StartThunder()
        {
            StartCoroutine(StormRoutine());
        }

        private IEnumerator StormRoutine()
        {
            // 1. Configurar Audio Source
            thunderSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Environment);
            audioSourceState = thunderSource.GetSnapshot();

            while (true)
            {
                // 2. Esperar tiempo aleatorio para el próximo rayo
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                // 3. Determinar qué tan "lejos" cayó (simulado)
                // 0 = muy cerca (trueno inmediato), 1 = lejos (trueno con delay)
                float distanceFactor = Random.Range(0.1f, 1.0f); 

                // 4. Ejecutar el Flash Visual (Luz + Rayo en el cielo)
                yield return StartCoroutine(FlashRoutine(distanceFactor));

                // 5. Ejecutar el Sonido (con delay basado en distancia)
                StartCoroutine(ThunderAudioRoutine(distanceFactor));
            }
        }

        private IEnumerator FlashRoutine(float distanceFactor)
        {
            // Opcional: Mover y mostrar el rayo visual
            if (lightningBoltObject != null && skyPositions.Length > 0)
            {
                // Elegir posición random
                Transform spawnPos = skyPositions[Random.Range(0, skyPositions.Length)];
                lightningBoltObject.transform.position = spawnPos.position;
                lightningBoltObject.transform.rotation = spawnPos.rotation;
                
                // Variar escala o rotación para que no parezca siempre el mismo
                lightningBoltObject.transform.localScale = Vector3.one * Random.Range(0.8f, 1.5f);

                lightningBoltObject.SetActive(true);
            }

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
                    lightningLight.intensity = curveVal * maxIntensity * (1.0f - distanceFactor);
                }

                yield return null;
            }

            // Asegurar apagado
            if (lightningLight != null) lightningLight.intensity = 0f;
            if (lightningBoltObject != null) lightningBoltObject.SetActive(false);
        }

        private IEnumerator ThunderAudioRoutine(float distanceFactor)
        {
            // Calculamos el delay: Si está cerca, delay casi 0. Si está lejos, delay alto.
            float delay = distanceFactor * speedOfSoundDelay; // Ej: 1.0 * 3 segundos = 3s delay

            yield return new WaitForSeconds(delay);

            if (thunderSoundClip != null && thunderSource != null)
            {
                // Tip de realismo: Si está lejos, suena más despacio y grave (low pass filter sería ideal, pero volumen sirve)
                thunderSource.clip = thunderSoundClip;
                thunderSource.transform.position = lightningBoltObject.transform.position;
                thunderSource.maxDistance = 150f;
                thunderSource.pitch = Random.Range(0.9f, 1.15f);
                thunderSource.volume = Mathf.Lerp(1.0f, 0.4f, distanceFactor); // Más lejos = menos volumen
                thunderSource.Play();
                yield return new WaitUntil(() => !thunderSource.isPlaying);
                AudioUtils.StopAndRestoreAudioSource(thunderSource, audioSourceState);
            }
        }
    }
}