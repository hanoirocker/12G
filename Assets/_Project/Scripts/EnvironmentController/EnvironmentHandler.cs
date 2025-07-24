namespace TwelveG.EnvironmentController
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EnvironmentHandler : MonoBehaviour
    {
        [Header("Small lights")]

        private int currentScene;

        void Start()
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
            VerifySceneEnvironmentConfigs();
        }

        private void VerifySceneEnvironmentConfigs()
        {
            switch (currentScene)
            {
                case 0: // Intro
                    // SetIntroConfigs();
                    break;
                case 1: // Main Menu
                    // SetMainMenuConfigs();
                    break;
                case 2: // Afternoon
                    // SetAfternoonConfigs();
                    break;
                case 3: // Evening
                    // SetEveningConfigs();
                    break;
                case 4: // Night
                    // SetNightConfigs();
                    break;
            }
        }

        private void SetEveningConfigs()
        {
        }

        private void SetNightConfigs()
        {

        }
    }
}