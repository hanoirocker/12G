namespace TwelveG.Environment
{
    using System;
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
                case 0: // Afternoon
                    // SetAfternoonConfigs();
                    break;
                case 1: // Evening
                    // SetEveningConfigs();
                    break;
                case 2: // Night
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