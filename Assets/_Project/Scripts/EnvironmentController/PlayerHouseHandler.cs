using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.InteractableObjects;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public enum HouseArea
    {
        PlayerBedroom,
        GuestsBathroom,
        GuestsBedroom,
        ParentsBedroom,
        Zoom,
        ParentsBathroom,
        UpstairsHall,
        MiddleStairs,
        LivingRoom,
        Kitchen,
        KitchenDepot,
        DownstairsHall,
        DownstairsOffice,
        Entrance,
        Garage,
        LowerStairs,
        UpperStairs,
        None
    }

    public enum HouseObjects
    {
        EntranceMainDoor,
        GarageNoise,
        GarageGate,
        GarageDoor,
        KitchenKnifeStack,
        KitchenTable,
        KitchenDepotDoor
    }

    public class PlayerHouseHandler : MonoBehaviour
    {
        public static PlayerHouseHandler Instance { get; private set; }

        [Header("Prefab References")]
        [Tooltip("Prefabs that will be enabled/disabled based on checkpoint data")]
        [SerializeField] private GameObject[] storedPrefabs;
        [Space(5)]
        [SerializeField] private DownstairsOfficeDoorHandler[] lockeableDoorsInHouse;
        [Space(5)]
        [SerializeField] private Transform entranceMainDoorTransform;
        [SerializeField] private Transform garageNoiseTransform;
        [SerializeField] private Transform garageDoorTransform;
        [SerializeField] private Transform garageGateTransform;
        [SerializeField] private Transform kitchenKnifeStackTransform;
        [SerializeField] private Transform kitchenTableTransform;
        [SerializeField] private Transform kitchenDepotDoorTransform;

        [Header("Light References")]
        [SerializeField] private Light[] HouseLights;
        [Space(5)]
        [SerializeField] private Renderer[] HouseLightsBulbs;
        [Space(5)]
        [SerializeField] private LightSwitchHandler[] LightSwitches;
        [Space(5)]
        [SerializeField] private Collider[] electricInteractableColliders;

        [Space(10)]
        [Header("Audio References")]
        [SerializeField] private GameObject acousticZonesParent;

        [SerializeField, Range(0f, 15f)] private float defaultDelay = 0.3f;

        // Diccionario extras para optimizar la búsqueda de prefabs por ID
        private Dictionary<string, GameObject> objectsMap = new Dictionary<string, GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // --- INICIALIZACIÓN DEL DICCIONARIO ---
            InitializeObjectsMap();
        }

        private void InitializeObjectsMap()
        {
            objectsMap.Clear();
            foreach (var prefab in storedPrefabs)
            {
                if (prefab != null)
                {
                    if (!objectsMap.ContainsKey(prefab.name))
                    {
                        objectsMap.Add(prefab.name, prefab);
                    }
                    else
                    {
                        Debug.LogWarning($"[PlayerHouseHandler] DUPLICADO DETECTADO: El objeto '{prefab.name}' está dos veces en la lista. Se ignoró el segundo.");
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

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

            if ((bool)data == false)
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

        public Transform GetTransformByObject(HouseObjects houseObject)
        {
            switch (houseObject)
            {
                case HouseObjects.EntranceMainDoor:
                    return entranceMainDoorTransform;
                case HouseObjects.GarageNoise:
                    return garageNoiseTransform;
                case HouseObjects.GarageGate:
                    return garageGateTransform;
                case HouseObjects.GarageDoor:
                    return garageDoorTransform;
                case HouseObjects.KitchenKnifeStack:
                    return kitchenKnifeStackTransform;
                case HouseObjects.KitchenTable:
                    return kitchenTableTransform;
                case HouseObjects.KitchenDepotDoor:
                    return kitchenDepotDoorTransform;
                default:
                    Debug.LogWarning("Objeto de casa inválido.");
                    return null;
            }
        }

        public void UnlockAllLockedDoors()
        {
            foreach (ICheckpointListener door in lockeableDoorsInHouse)
            {
                door.OnCheckpointReached("UNLOCKED");
            }
        }

        public GameObject GetStoredObjectByID(string objectID)
        {
            if (objectsMap.TryGetValue(objectID, out GameObject foundObject))
            {
                return foundObject;
            }

            Debug.LogWarning($"[PlayerHouseHandler] Objeto con ID '{objectID}' no encontrado en el Mapa.");
            return null;
        }

        public void ToggleStoredPrefabs(ObjectData objectData)
        {
            if (objectsMap.TryGetValue(objectData.objectID, out GameObject foundObject))
            {
                // Solo activar/desactivar si el estado es diferente
                if (foundObject.activeSelf != objectData.isActive)
                {
                    foundObject.SetActive(objectData.isActive);
                }
            }
            else
            {
                Debug.LogWarning($"[PlayerHouseHandler] No se pudo hacer Toggle. ID '{objectData.objectID}' desconocido.");
            }
        }
    }
}