using UnityEngine;

namespace TwelveG.AudioController
{
    // Parametros de un AudioSource que se pueden guardar y restaurar
    public struct AudioSourceState
    {
        public float volume;
        public float pitch;
        public float spatialBlend;
        public float dopplerLevel;
        public float spread;
        public float minDistance;
        public float maxDistance;
        public AudioRolloffMode rolloffMode;
        public int priority;
        public bool loop;
        public UnityEngine.Audio.AudioMixerGroup outputAudioMixerGroup;
        
        public Vector3 localPosition;
        public Vector3 globalPosition;
        public Quaternion localRotation;
        public Quaternion globalRotation;
    }

    public static class AudioExtensions
    {
        public static AudioSourceState GetSnapshot(this AudioSource source)
        {
            return new AudioSourceState
            {
                volume = source.volume,
                pitch = source.pitch,
                spatialBlend = source.spatialBlend,
                dopplerLevel = source.dopplerLevel,
                spread = source.spread,
                minDistance = source.minDistance,
                maxDistance = source.maxDistance,
                rolloffMode = source.rolloffMode,
                priority = source.priority,
                loop = source.loop,
                outputAudioMixerGroup = source.outputAudioMixerGroup,
                
                globalPosition = source.transform.position,
                globalRotation = source.transform.rotation,
                localPosition = source.transform.localPosition,
                localRotation = source.transform.localRotation
            };
        }

        public static void RestoreSnapshot(this AudioSource source, AudioSourceState state)
        {
            source.volume = state.volume;
            source.pitch = state.pitch;
            source.spatialBlend = state.spatialBlend;
            source.dopplerLevel = state.dopplerLevel;
            source.minDistance = state.minDistance;
            source.maxDistance = state.maxDistance;
            source.rolloffMode = state.rolloffMode;
            source.spread = state.spread;
            source.priority = state.priority;
            source.loop = state.loop;
            source.outputAudioMixerGroup = state.outputAudioMixerGroup;

            source.transform.SetPositionAndRotation(state.globalPosition, state.globalRotation);
            source.transform.SetLocalPositionAndRotation(state.localPosition, state.localRotation);
        }
    }
}