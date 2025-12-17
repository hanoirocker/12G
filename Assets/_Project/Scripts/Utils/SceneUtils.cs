using System.Diagnostics;
using UnityEngine.SceneManagement;

namespace TwelveG.GameController
{
    public enum SceneEnum
    {
        Afternoon,
        Evening,
        Night,
        MenuAfternoon,
        MenuEvening,
        MenuNight,
        Intro,
        Credits,
        None
    }

    public static class SceneUtils
    {
        public static SceneEnum RetrieveCurrentSceneEnum()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            switch (currentSceneName)
            {
                case "Afternoon Scene":
                    return SceneEnum.Afternoon;
                case "Menu Afternoon":
                    return SceneEnum.MenuAfternoon;
                case "Evening Scene":
                    return SceneEnum.Evening;
                case "Menu Evening":
                    return SceneEnum.MenuEvening;
                case "Night Scene":
                    return SceneEnum.Night;
                case "Menu Night":
                    return SceneEnum.MenuNight;
                default:
                    Debug.Assert(false, $"SceneEnum no definido para la escena actual: {currentSceneName}");
                    return SceneEnum.None;
            }
        }
    }
}