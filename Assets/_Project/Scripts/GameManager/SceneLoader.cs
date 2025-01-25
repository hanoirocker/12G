namespace GameSceneManager
{
    using UnityEngine.SceneManagement;
    using UnityEngine;

    public class SceneLoader : MonoBehaviour
    {
        public void NextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public int GetActualScene()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }
}