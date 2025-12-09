namespace TwelveG.EnvironmentController
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PoliceCarHandler : MonoBehaviour
    {
        [Header("References")]
        [Space]
        [SerializeField] private VehicleType vehicleType;
        [SerializeField] List<Light> carLights = new List<Light>();
        [SerializeField] private float lightToggleTime = 0.1f;

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
            audioSource.Stop();
            audioSource.clip = null;

            // Activamos el efecto de particulas de la explosion y el humo
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Play();
            }

            yield return new WaitForSeconds(20f);
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }
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