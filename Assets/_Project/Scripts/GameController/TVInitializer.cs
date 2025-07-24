namespace TwelveG.GameController
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
            if (SceneManager.GetActiveScene().buildIndex == 3) // Evening
            {
                tvEvening.SetActive(true);
            }
        }
    }

}