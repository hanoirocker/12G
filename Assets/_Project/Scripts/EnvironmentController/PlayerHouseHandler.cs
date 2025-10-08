namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerHouseHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Light[] HouseLights;
        

        public void FlickerLights(Component sender, object data)
        {
            StartCoroutine(FlickerSequence((float)data));
        }

        private IEnumerator FlickerSequence(float delay)
        {
            yield return new WaitForSeconds(delay);

            Dictionary<Light, bool> originalStates = new Dictionary<Light, bool>();
            foreach (Light light in HouseLights)
            {
                originalStates[light] = light.enabled;
            }

            foreach (Light light in HouseLights)
            {
                if (originalStates[light])
                    light.enabled = false;
            }
            yield return new WaitForSeconds(0.2f);

            foreach (Light light in HouseLights)
            {
                if (originalStates[light])
                    light.enabled = true;
            }
            yield return new WaitForSeconds(0.15f);

            foreach (Light light in HouseLights)
            {
                if (originalStates[light])
                    light.enabled = false;
            }

            yield return new WaitForSeconds(0.1f);

            foreach (Light light in HouseLights)
            {
                light.enabled = originalStates[light];
            }
        }
    }
}