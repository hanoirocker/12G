namespace TwelveG.AudioManager
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EnvironmentAudioController : MonoBehaviour
    {
        [SerializeField] private GameObject afternoonSounds;
        [SerializeField] private GameObject eveningSounds;
        [SerializeField] private GameObject nightSounds;

        private int currentScene;

        void Start()
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
            VerifySceneEnvironmentSounds();
        }

        private void VerifySceneEnvironmentSounds()
        {
            switch (currentScene)
            {
                case (0):
                    afternoonSounds.SetActive(true);
                    eveningSounds.SetActive(false);
                    nightSounds.SetActive(false);
                    break;
                case (1):
                    afternoonSounds.SetActive(false);
                    nightSounds.SetActive(false);
                    break;
                case (2):
                    afternoonSounds.SetActive(false);
                    eveningSounds.SetActive(false);
                    break;
            }
        }
    }

}