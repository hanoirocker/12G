namespace TwelveG.AudioController
{
    using System.Collections.Generic;
    using UnityEngine;

    public class EnvironmentAudioHandler : MonoBehaviour
    {
        [Header("Scene-based Ambient Prefabs")]
        [SerializeField] private List<Transform> afternoonSoundsTransforms;
        [SerializeField] private List<Transform> eveningSoundsTransforms;
        [SerializeField] private List<Transform> nightSoundsTransforms;

        // Llamado desde AudioManager
        public void InitiateEnvironmentSounds(float sceneIndex)
        {
            // Dependiendo de la escena, debe comenzar a reproducir sonidos de la misma
            // basado en Transforms para cada escena.
        }
    }
}
