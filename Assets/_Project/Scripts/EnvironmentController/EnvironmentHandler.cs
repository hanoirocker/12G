using System.Collections;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public enum EnemyPositions
    {
        None,
        PlayerHouseCorner,
        MiddleOfTheStreet,
        LivingRoomRightWindow,
        DownstairsHallWindow
    }
    
    public class EnvironmentHandler : MonoBehaviour
    {
        public static EnvironmentHandler Instance { get; private set; }

        [Header("References")]
        [Space]
        [SerializeField] private GameObject rainObject;
        [SerializeField] private LightningStormHandler lightningStormHandler;
        [SerializeField] private GameObject windZoneObject;
        [SerializeField] private WindZone windZone;

        [Space(10)]
        [Header("Enemy References")]
        [Space(5)]
        [SerializeField] GameObject enemyPrefab;
        [SerializeField] private Animation animationComponent;

        [Space(5)]
        [Header("Transforms")]
        [SerializeField] private Transform cornerTransform;
        [SerializeField] private Transform middleOfTheStreetTransform;
        [SerializeField] private Transform livingRoomRightWindowTransform;
        [SerializeField] private Transform downstairsHallWindowTransform;

        [Space(10)]
        [Header("Prefab References")]
        [SerializeField] private GameObject[] storedPrefabs;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
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

        public void ToggleStoredPrefabs(ObjectData objectData)
        {
            if (storedPrefabs == null || storedPrefabs.Length == 0)
            {
                return;
            }

            foreach (GameObject prefab in storedPrefabs)
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

        public IEnumerator PlayEnemyAnimation(string animationName, bool deactivateAfter)
        {
            animationComponent.Play(animationName);

            if (deactivateAfter)
            {
                yield return new WaitForSeconds(animationComponent[animationName].length);
                enemyPrefab.SetActive(false);
            }
            else
            {
                yield return null;
            }
        }

        public void ShowEnemy(EnemyPositions position)
        {
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
                case EnemyPositions.LivingRoomRightWindow:
                    enemyPrefab.transform.position = livingRoomRightWindowTransform.position;
                    enemyPrefab.transform.rotation = livingRoomRightWindowTransform.rotation;
                    break;
                case EnemyPositions.DownstairsHallWindow:
                    enemyPrefab.transform.position = downstairsHallWindowTransform.position;
                    enemyPrefab.transform.rotation = downstairsHallWindowTransform.rotation;
                    break;
                case EnemyPositions.None:
                    enemyPrefab.SetActive(false);
                    return;
                default:
                    Debug.LogWarning("Posición de enemigo inválida especificada.");
                    return;
            }

            enemyPrefab.SetActive(true);
            enemyPrefab.GetComponent<ZoneSpotterHandler>().canBeSpotted = true;
        }
    }
}