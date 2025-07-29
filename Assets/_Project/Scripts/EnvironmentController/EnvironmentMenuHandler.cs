namespace TwelveG.EnvironmentController
{
    using TwelveG.SaveSystem;
    using UnityEngine;

    public class EnvironmentMenuHandler : MonoBehaviour, IDataPersistence
    {
        [Header("Scene References")]
        [SerializeField] private GameObject directionalLightObj;
        [SerializeField] private GameObject flogPlanes;
        [SerializeField] private GameObject rainFX;
        [SerializeField] private GameObject streetLightsOn;
        [SerializeField] private GameObject housesLightsOn;

        [Header("Skybox Materials by Scene")]
        [SerializeField] private Material skyboxAfternoon;
        [SerializeField] private Material skyboxEvening;
        [SerializeField] private Material skyboxNight;

        [Header("Sun Vectors by Scene")]
        [SerializeField] private Vector3 sunRotScene2;
        [SerializeField] private Vector3 sunRotScene3;
        [SerializeField] private Vector3 sunRotScene4;

        [Header("Realtime Sun Colors")]
        [SerializeField] private Color sunColorScene2;
        [SerializeField] private Color sunColorScene3;

        [Header("Realtime Shadow Colors")]
        [SerializeField] private Color shadowColorScene2;
        [SerializeField] private Color shadowColorScene3;
        [SerializeField] private Color shadowColorScene4;

        [Header("Fog Colors")]
        [SerializeField] private Color fogColorScene2;
        [SerializeField] private Color fogColorScene3;
        [SerializeField] private Color fogColorScene4;

        private Light directionalLight;
        private Transform directionalLightTransform;

        private void Awake()
        {
            directionalLight = directionalLightObj.GetComponent<Light>();
            directionalLightTransform = directionalLightObj.GetComponent<Transform>();
        }

        private void OnLastSceneSavedSettings(GameData data)
        {
            print("data.sceneIndex: " + data.sceneIndex);
            switch (data.sceneIndex)
            {
                case 2:
                    directionalLight.color = sunColorScene2;
                    directionalLightTransform.rotation = Quaternion.Euler(sunRotScene2);
                    RenderSettings.skybox = skyboxAfternoon;
                    RenderSettings.subtractiveShadowColor = shadowColorScene2;
                    RenderSettings.fogColor = fogColorScene2;
                    break;

                case 3:
                    directionalLight.color = sunColorScene3;
                    directionalLightTransform.rotation = Quaternion.Euler(sunRotScene3);
                    RenderSettings.skybox = skyboxEvening;
                    RenderSettings.subtractiveShadowColor = shadowColorScene3;
                    RenderSettings.fogColor = fogColorScene3;
                    break;

                case 4:
                    streetLightsOn.SetActive(true);
                    housesLightsOn.SetActive(true);
                    directionalLightObj.SetActive(false);
                    flogPlanes.SetActive(true);
                    rainFX.SetActive(true);
                    directionalLightTransform.rotation = Quaternion.Euler(sunRotScene4);
                    RenderSettings.skybox = skyboxNight;
                    RenderSettings.ambientLight = shadowColorScene4;
                    RenderSettings.fogColor = fogColorScene4;
                    break;

                default:
                    // Opcional: valores por defecto
                    break;
            }

            // DynamicGI.UpdateEnvironment();
        }

        public void LoadData(GameData data)
        {
            OnLastSceneSavedSettings(data);
        }

        public void SaveData(ref GameData data)
        {
            return;
        }
    }
}