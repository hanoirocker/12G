namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PoliceCarHandler : MonoBehaviour
    {
        [Header("Vehicle Settings")]
        [Space]
        [SerializeField] private VehicleType vehicleType;
        [Header("Vehicle Settings")]
        [Space]
        [SerializeField] List<Light> carLights = new List<Light>();
        [SerializeField] private float lightToggleTime = 0.1f;
        [Header("Particle Settings")]
        [Space]
        [SerializeField] GameObject particleFX;

        private Coroutine sirenLightsRoutine;

        private void Start()
        {
            if (vehicleType == VehicleType.RegularPoliceCar)
            {
                StartCoroutine(SirenLightsRoutine());
            }
            else if (vehicleType == VehicleType.PoliceCarCrash)
            {
                StartCoroutine(CrashSequenceRoutine());
            }
        }

        private IEnumerator CrashSequenceRoutine()
        {
            Animation animationComponent = GetComponent<Animation>();
            AudioSource audioSource = GetComponent<AudioSource>();
            sirenLightsRoutine = StartCoroutine(SirenLightsRoutine());
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            // El auto choca contra la escuela
            StopCoroutine(sirenLightsRoutine);
            foreach (Light light in carLights)
            {
                light.enabled = false;
            }

            // Activamos el efecto de particulas de la explosion y el humo
            particleFX.SetActive(true);
            yield return new WaitForSeconds(900f);
            audioSource.Stop();
            particleFX.SetActive(false);
        }

        private IEnumerator SirenLightsRoutine()
        {
            while (true)
            {
                foreach (Light light in carLights)
                {
                    light.enabled = !light.enabled;
                }
                yield return new WaitForSeconds(lightToggleTime);
            }
        }
    }
}