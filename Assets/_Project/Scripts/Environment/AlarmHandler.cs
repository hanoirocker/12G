namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AlarmHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Renderer> carLightsRenderers;
        [SerializeField] private AudioSource alarmAudioSource;
        [SerializeField] private AudioSource shotGunAudioSource;

        [Header("Alarm Settings")]
        [SerializeField, Range(5f, 20f)] private float alarmDurantion = 8f;
        [SerializeField] private float lightToggleTime = 0.48f;

        [Header("EventSOs settings")]
        public GameEventSO carAlarmStopped;

        private bool lightsAreOff = true;
        private AudioSource audioSource;
        private List<Material> carLightsMaterials;

        public void TriggerAlarm()
        {
            StartCoroutine(AlarmCoroutine());
        }

        IEnumerator AlarmCoroutine()
        {
            shotGunAudioSource.Play();

            yield return new WaitForSeconds(1.5f);

            alarmAudioSource.Play();

            Light[] lights = GetComponentsInChildren<Light>();

            float timePassed = 0;
            while (timePassed < alarmDurantion)
            {
                foreach (Light light in lights)
                {
                    light.enabled = !light.enabled;
                }

                lightsAreOff = !lightsAreOff;

                foreach (Renderer lightRenderer in carLightsRenderers)
                {
                    Material carLightMaterial = lightRenderer.material;

                    if (lightsAreOff)
                    {
                        carLightMaterial.DisableKeyword("_EMISSION");
                    }
                    else
                    {
                        carLightMaterial.EnableKeyword("_EMISSION");
                    }
                }
                yield return new WaitForSeconds(lightToggleTime);
                timePassed += lightToggleTime;
            }

            alarmAudioSource.Stop();

            // Destroy(gameObject.GetComponent<GameEventListener>());
            // Destroy(shotGunAudioSource);
            // Destroy(alarmAudioSource);
            // audioSource = null;
            // shotGunAudioSource = null;
            // carAlarmStopped.Raise(this, null);
            // Destroy(this);
        }
    }
}