namespace TwelveG.EnvironmentController
{
    using TwelveG.AudioController;
    using UnityEngine;

    public class EnvironmentHandler : MonoBehaviour
    {
        [Header("References")]
        [Space]
        [SerializeField] private GameObject rainObject;
        [SerializeField] private GameObject windZoneObject;
        [SerializeField] private WindZone windZone;

        public void EnvironmentWeatherConfig(Component sender, object data)
        {
            switch ((WeatherEvent)data)
            {
                case (WeatherEvent.SoftRain):
                    rainObject.SetActive(true);
                    break;
                case (WeatherEvent.HardRain):
                    rainObject.SetActive(true);
                    Debug.Log($"Aun no hay diferencia entre soft rain y hard rain!");
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
                default:
                    break;
            }
        }
    }
}