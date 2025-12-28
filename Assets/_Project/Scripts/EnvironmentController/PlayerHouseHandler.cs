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
        [SerializeField] private GameObject[] checkpointPrefabs;

        [Header("Light References")]
        [SerializeField] private Light[] HouseLights;
        [SerializeField] private LightSwitchHandler[] LightSwitches;
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

            foreach (Collider col in electricInteractableColliders)
            {
                col.enabled = (bool)data;
            }
        }

        public void ToggleCheckpointPrefabs(Component sender, object data)
        {
            if (checkpointPrefabs == null || checkpointPrefabs.Length == 0)
            {
                Debug.LogWarning("No checkpoint prefabs assigned in PlayerHouseHandler.");
                return;
            }

            ObjectData objectData = (ObjectData)data;

            foreach (GameObject prefab in checkpointPrefabs)
            {
                if (prefab.name == objectData.objectID)
                {
                    Debug.Log("Toggling prefab: " + prefab.name + " to " + objectData.isActive);
                    prefab.SetActive(objectData.isActive);
                    break;
                }
            }
        }
    }
}