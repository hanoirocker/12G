using UnityEngine;
using TwelveG.EnvironmentController; // Para el enum HouseArea

namespace TwelveG.AudioController
{
    // 1. Estructura para configurar CADA clip individualmente
    [System.Serializable]
    public struct AmbienceClipConfig
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume; // Volumen específico para este clip en esta área
    }

    // 2. El Scriptable Object que agrupa los sonidos de un área
    [CreateAssetMenu(fileName = "AmbienceProfileSO - ", menuName = "SO's/Data Structures/Ambience Profile")]
    public class AmbienceProfileSO : ScriptableObject
    {
        [Header("Trigger Areas")]
        public HouseArea[] activeAreas;

        [Header("Audio Configuration")]
        public System.Collections.Generic.List<AmbienceClipConfig> ambientClips;
    }
}