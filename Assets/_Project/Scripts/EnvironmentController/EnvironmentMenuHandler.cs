namespace TwelveG.EnvironmentController
{
    using TwelveG.SaveSystem;
    using UnityEngine;

    public class EnvironmentMenuHandler : MonoBehaviour, IDataPersistence
    {
        [Header("Scene References")]
        [SerializeField] private Transform sunTransform;
        [SerializeField] private GameObject flogPlanes;
        [SerializeField] private GameObject rainFX;

        [Header("Skybox Materials by Scene")]
        [SerializeField] private Material skyboxAfternoon;
        [SerializeField] private Material skyboxEvening;
        [SerializeField] private Material skyboxNight;

        [Header("Sun Vectors by Scene")]
        [SerializeField] private Vector3 sunRotScene2;
        [SerializeField] private Vector3 sunRotScene3;
        [SerializeField] private Vector3 sunRotScene4;

        [Header("Realtime Shadow Colors")]
        [SerializeField] private Color shadowColorScene2;
        [SerializeField] private Color shadowColorScene3;
        [SerializeField] private Color shadowColorScene4;

        [Header("Fog Colors")]
        [SerializeField] private Color fogColorScene2;
        [SerializeField] private Color fogColorScene3;
        [SerializeField] private Color fogColorScene4;

        // Valores de posición y rotación para cada escena
        // private readonly Vector3 sunPosScene2 = new Vector3(10f, 20f, 0f);
        // private readonly Vector3 sunRotScene2 = new Vector3(50f, 0f, 0f);

        // private readonly Vector3 sunPosScene3 = new Vector3(0f, 30f, -10f);
        // private readonly Vector3 sunRotScene3 = new Vector3(75f, 45f, 0f);

        // private readonly Vector3 sunPosScene4 = new Vector3(-10f, 15f, 5f);
        // private readonly Vector3 sunRotScene4 = new Vector3(90f, 90f, 0f);

        private void OnLastSceneSavedSettings(GameData data)
        {
            print("data.sceneIndex: " + data.sceneIndex);
            switch (data.sceneIndex)
            {
                case 2:
                    sunTransform.rotation = Quaternion.Euler(sunRotScene2);
                    RenderSettings.skybox = skyboxAfternoon;
                    RenderSettings.subtractiveShadowColor = shadowColorScene2;
                    RenderSettings.fogColor = fogColorScene2;
                    break;

                case 3:
                    sunTransform.rotation = Quaternion.Euler(sunRotScene3);
                    RenderSettings.skybox = skyboxEvening;
                    RenderSettings.subtractiveShadowColor = shadowColorScene3;
                    RenderSettings.fogColor = fogColorScene3;
                    break;

                case 4:
                    flogPlanes.SetActive(true);
                    rainFX.SetActive(true);
                    sunTransform.rotation = Quaternion.Euler(sunRotScene4);
                    RenderSettings.skybox = skyboxNight;
                    RenderSettings.ambientLight = shadowColorScene4;
                    RenderSettings.fogColor = fogColorScene4;
                    break;

                default:
                    // Opcional: valores por defecto
                    break;
            }

            if (sunTransform.TryGetComponent<Light>(out var directionalLight))
            {
                RenderSettings.sun = directionalLight;
            }

            DynamicGI.UpdateEnvironment();
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