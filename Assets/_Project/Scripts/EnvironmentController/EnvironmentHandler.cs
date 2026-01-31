using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class EnvironmentHandler : MonoBehaviour
    {
        public static EnvironmentHandler Instance { get; private set; }

        [Header("References")]
        [Space]
        public GameObject Enemy;
        [SerializeField] private EnemyHandler enemyHandler;
        public EnemyHandler EnemyHandler => enemyHandler;

        [SerializeField] private GameObject rainObject;
        [SerializeField] private LightningStormHandler lightningStormHandler;
        [SerializeField] private GameObject windZoneObject;
        [SerializeField] private WindZone windZone;

        [Space(10)]
        [Header("Prefab References")]
        [Tooltip("Arrastra aquí los objetos. El nombre del GameObject en la escena será su ID.")]
        [SerializeField] private GameObject[] storedPrefabs;

        private Dictionary<string, GameObject> prefabsDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePrefabsDictionary();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void InitializePrefabsDictionary()
        {
            prefabsDictionary = new Dictionary<string, GameObject>();

            if (storedPrefabs == null) return;

            foreach (var prefab in storedPrefabs)
            {
                if (prefab == null) continue;

                if (!prefabsDictionary.ContainsKey(prefab.name))
                {
                    prefabsDictionary.Add(prefab.name, prefab);
                }
                else
                {
                    Debug.LogWarning($"[EnvironmentHandler] ID Duplicado encontrado: '{prefab.name}'. Se omitirá el duplicado.");
                }
            }
        }

        public void EnvironmentWeatherConfig(Component sender, object data)
        {
            switch ((WeatherEvent)data)
            {
                case (WeatherEvent.ConstantThunders):
                    lightningStormHandler.StartConstantThunder();
                    break;
                case (WeatherEvent.CloseThunder):
                    lightningStormHandler.StartCloseThunder();
                    break;
                case (WeatherEvent.SoftWind):
                    windZone.windMain = 0.2f;
                    windZone.windTurbulence = 0.2f;
                    windZone.windPulseFrequency = 0.2f;
                    windZone.windPulseMagnitude = 0.2f;
                    windZoneObject.SetActive(true);
                    break;
                case (WeatherEvent.HardWind):
                    windZone.windMain = 1f;
                    windZone.windTurbulence = 0.3f;
                    windZone.windPulseFrequency = 0.2f;
                    windZone.windPulseMagnitude = 0.7f;
                    windZoneObject.SetActive(true);
                    break;
                case (WeatherEvent.SoftRain):
                    rainObject.SetActive(true);
                    break;
                case (WeatherEvent.HardRain):
                    rainObject.SetActive(true);
                    Debug.Log($"Aun no hay diferencia de VFX particule entre soft rain y hard rain!");
                    break;
                case (WeatherEvent.HardRainAndWind):
                    rainObject.SetActive(true);
                    windZone.windMain = 1f;
                    windZone.windTurbulence = 0.3f;
                    windZone.windPulseFrequency = 0.2f;
                    windZone.windPulseMagnitude = 0.7f;
                    windZoneObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public GameObject GetStoredObjectByID(string objectID)
        {
            if (prefabsDictionary.TryGetValue(objectID, out GameObject foundObject))
            {
                return foundObject;
            }

            Debug.LogWarning($"[EnvironmentHandler] Objeto con ID '{objectID}' no encontrado en el Mapa.");
            return null;
        }

        public void ToggleStoredPrefabs(ObjectData objectData)
        {
            if (prefabsDictionary == null) return;

            if (prefabsDictionary.TryGetValue(objectData.objectID, out GameObject targetPrefab))
            {
                if (targetPrefab != null)
                {
                    targetPrefab.SetActive(objectData.isActive);
                }
            }
            else
            {
                Debug.LogWarning($"[EnvironmentHandler] No se encontró ningún prefab con el ID: {objectData.objectID}");
            }
        }
    }
}