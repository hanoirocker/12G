using System.Collections;
using System.Collections.Generic;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class PoliceCarHandler : MonoBehaviour
    {
        [Header("Vehicle Settings")]
        [SerializeField] private VehicleType vehicleType;
        [SerializeField] List<Light> carLights = new List<Light>();
        [SerializeField] private float lightToggleTime = 0.1f;
        [SerializeField, Range(0f, 20f)] private float lightsDurationAfterCrash = 20f;

        [Space(10)]
        [Header("Particle Settings")]
        [Space]
        [SerializeField] GameObject particleFX;
        [SerializeField, Range(0f, 900f)] private float particleDuration = 900f;

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
            // Activar collider de objeto spotteable
            Collider collider = GetComponentInChildren<ZoneSpotterHandler>().gameObject.GetComponent<Collider>();
            collider.enabled = true;

            // Activamos el efecto de particulas de la explosion y el humo
            particleFX.SetActive(true);

            // Esperar para apagar las luces
            yield return new WaitForSeconds(lightsDurationAfterCrash);
            StopCoroutine(sirenLightsRoutine);
            foreach (Light light in carLights)
            {
                light.enabled = false;
            }

            audioSource.Stop();

            // Esperar para apagar los efectos de particulas
            yield return new WaitForSeconds(particleDuration);
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