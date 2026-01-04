using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.InteractableObjects;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class PlayerHouseHandler : MonoBehaviour
    {
        [Header("Prefab References")]
        [Tooltip("Prefabs that will be enabled/disabled based on checkpoint data")]
        [SerializeField] private GameObject[] checkpointPrefabs;

        [Header("Light References")]
        [SerializeField] private Light[] HouseLights;
        [SerializeField] private Renderer[] HouseLightsBulbs;
        [Space(5)]
        [SerializeField] private LightSwitchHandler[] LightSwitches;
        [Space(5)]
        [SerializeField] private Collider[] electricInteractableColliders;

        [Space]
        [Header("Audio References")]
        [SerializeField] private GameObject acousticZonesParent;

        [SerializeField, Range(0f, 15f)] private float defaultDelay = 0.3f;

        public void FlickerLights(Component sender, object data)
        {
            if (data != null)
            {
                StartCoroutine(FlickerSequence((float)data));
            }
            else
            {
                StartCoroutine(FlickerSequence(defaultDelay));
            }
        }

        public void ToggleAcousticZones(Component sender, object data)
        {
            WeatherEvent weatherEvent = (WeatherEvent)data;

            if (weatherEvent == WeatherEvent.None)
            {
                acousticZonesParent.SetActive(false);
            }
            else
            {
                acousticZonesParent.SetActive(true);
            }
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
            yield return new WaitForSeconds(0.1f);

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

        public void EnablePlayerHouseEnergy(Component sender, object data)
        {
            if (data == null) return;

            foreach (LightSwitchHandler lightSwitch in LightSwitches)
            {
                lightSwitch.itWorks = (bool)data;
            }

            if((bool)data == false)
            {
                // Apagar todas las luces de la casa
                foreach (Light light in HouseLights)
                {
                    light.enabled = false;
                }

                foreach (Renderer bulb in HouseLightsBulbs)
                {
                    bulb.material.DisableKeyword("_EMISSION");
                }
            }

            foreach (Collider col in electricInteractableColliders)
            {
                col.enabled = (bool)data;
            }
        }

        public void ToggleCheckpointPrefabs(ObjectData objectData)
        {
            if (checkpointPrefabs == null || checkpointPrefabs.Length == 0)
            {
                return;
            }

            foreach (GameObject prefab in checkpointPrefabs)
            {
                if (prefab.name == objectData.objectID)
                {
                    prefab.SetActive(objectData.isActive);
                    break;
                }
            }
        }
    }
}