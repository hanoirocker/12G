namespace TwelveG.AudioManager
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EnvironmentAudioController : MonoBehaviour
    {
        public static EnvironmentAudioController Instance;

        [Header("Ambience")]
        [SerializeField] private List<AudioSource> ambienceSource = new(); // Máximo 2 fuentes activas

        [Header("Scene-based Ambient Prefabs")]
        [SerializeField] private GameObject afternoonSounds;
        [SerializeField] private GameObject eveningSounds;
        [SerializeField] private GameObject nightSounds;

        // Historial de zonas activas recientes (máx 2)
        private LinkedList<Transform> activeZoneHistory = new();

        private int currentScene;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
            VerifySceneEnvironmentSounds();
        }

        // Se llama desde el AcousticZone script en OnTriggerEnter al colisionar
        // Ver: Acoustic Zones prefab en Player House prefab
        public void EnteredAcousticZone(Transform zoneTransform)
        {
            // Si ya estaba en la lista, la movemos al final (más reciente)
            if (activeZoneHistory.Contains(zoneTransform))
            {
                activeZoneHistory.Remove(zoneTransform);
            }

            activeZoneHistory.AddLast(zoneTransform);

            // Mantener máximo 2 zonas en la lista
            if (activeZoneHistory.Count > 2)
            {
                activeZoneHistory.RemoveFirst();
            }

            // Reasignar posiciones de los audio sources a las zonas más recientes
            int i = 0;
            foreach (var zone in activeZoneHistory)
            {
                if (i >= ambienceSource.Count) break;

                ambienceSource[i].transform.position = zone.position;

                if (!ambienceSource[i].isPlaying)
                    ambienceSource[i].Play();

                i++;
            }
        }

        public void ExitedAcousticZone(Transform zoneTransform)
        {
            if (activeZoneHistory.Contains(zoneTransform))
            {
                activeZoneHistory.Remove(zoneTransform);
            }
        }

        private void VerifySceneEnvironmentSounds()
        {
            switch (currentScene)
            {
                case 0:
                    afternoonSounds?.SetActive(true);
                    break;
                case 1:
                    eveningSounds?.SetActive(true);
                    break;
                case 2:
                    nightSounds?.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("[EnvironmentAudioController]: Unknown scene index.");
                    break;
            }
        }
    }
}
