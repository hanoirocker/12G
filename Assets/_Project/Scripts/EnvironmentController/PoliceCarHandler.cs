namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PoliceCarHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] List<Light> carLights = new List<Light>();
        [SerializeField] private float lightToggleTime = 0.1f;

        private void Start()
        {
            StartCoroutine(SirenLightsRoutine());
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