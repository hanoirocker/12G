using TwelveG.AudioController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public enum EnemyPositions
    {
        None,
        PlayerHouseCorner,
        MiddleOfTheStreet
    }

    public class EnvironmentHandler : MonoBehaviour
    {
        [Header("References")]
        [Space]
        [SerializeField] private GameObject rainObject;
        [SerializeField] private GameObject windZoneObject;
        [SerializeField] private WindZone windZone;

        [Space(10)]
        [Header("Enemy References")]
        [Space(5)]
        [SerializeField] GameObject enemyPrefab;
        [Space(5)]
        [SerializeField] private Transform cornerTransform;
        [SerializeField] private Transform middleOfTheStreetTransform;

        [Header("Prefab References")]
        [SerializeField] private GameObject[] checkpointPrefabs;

        public void EnvironmentWeatherConfig(Component sender, object data)
        {
            switch ((WeatherEvent)data)
            {
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
                    // Debug.Log("Toggling prefab: " + prefab.name + " to " + objectData.isActive);
                    prefab.SetActive(objectData.isActive);
                    break;
                }
            }
        }

        public Transform GetCurrentEnemyTransform()
        {
            Transform headTransform = enemyPrefab.transform.Find("Head");
            return headTransform;
        }

        public void OnShowEnemy(Component sender, object data)
        {
            EnemyPositions position = (EnemyPositions)data;

            switch (position)
            {
                case EnemyPositions.PlayerHouseCorner:
                    enemyPrefab.transform.position = cornerTransform.position;
                    enemyPrefab.transform.rotation = cornerTransform.rotation;
                    break;
                case EnemyPositions.MiddleOfTheStreet:
                    enemyPrefab.transform.position = middleOfTheStreetTransform.position;
                    enemyPrefab.transform.rotation = middleOfTheStreetTransform.rotation;
                    break;
                case EnemyPositions.None:
                    enemyPrefab.SetActive(false);
                    return;
                default:
                    Debug.LogWarning("Invalid enemy position specified.");
                    return;
            }

            enemyPrefab.SetActive(true);
        }
    }
}