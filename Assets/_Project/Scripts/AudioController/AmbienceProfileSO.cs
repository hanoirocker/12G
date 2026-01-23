using UnityEngine;
using TwelveG.EnvironmentController;

namespace TwelveG.AudioController
{
    [System.Serializable]
    public struct AmbienceClipConfig
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [CreateAssetMenu(fileName = "AmbienceProfileSO - ", menuName = "SO's/Data Structures/Ambience Profile")]
    public class AmbienceProfileSO : ScriptableObject
    {
        [Header("Trigger Areas")]
        public HouseArea[] activeAreas;

        [Header("Audio Configuration")]
        public System.Collections.Generic.List<AmbienceClipConfig> ambientClips;
    }
}