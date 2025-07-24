namespace TwelveG.GameController
{
    using UnityEngine;

    public class DependenciesManager : MonoBehaviour
    {
        public static DependenciesManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}