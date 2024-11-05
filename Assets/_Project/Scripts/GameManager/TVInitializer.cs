namespace TwelveG.GameManager
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class TVInitializer : MonoBehaviour
    {
        [SerializeField] GameObject tvEvening;

        void Start()
        {
            VerifySceneTV();
        }

        private void VerifySceneTV()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            if (currentScene == 1)
            {
                tvEvening.SetActive(true);
            }
        }
    }

}