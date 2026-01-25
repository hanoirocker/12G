using UnityEngine;

namespace TwelveG.InteractableObjects
{
    [RequireComponent(typeof(AudioSource))]
    public class WalkieTalkieAudioHandler : MonoBehaviour
    {
        [Header("Clips & Settings")]
        [SerializeField] private AudioClip channelSwitchClip;
        [SerializeField] private AudioClip incomingAlertClip;

        [Space]
        [SerializeField, Range(0f, 1f)] private float switchVolume = 0.8f;
        [SerializeField, Range(0f, 1f)] private float alertVolume = 0.7f;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayChannelSwitch()
        {
            if (audioSource.isPlaying) audioSource.Stop();

            audioSource.loop = false;
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(channelSwitchClip, switchVolume);
        }

        public void PlayFastSwitch(float pitch)
        {
            // Usado para el efecto de rebobinar rápido al canal de Mica
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(channelSwitchClip, switchVolume);
        }

        public void PlayIncomingAlert()
        {
            audioSource.Stop();
            audioSource.loop = true;
            audioSource.clip = incomingAlertClip;
            audioSource.volume = alertVolume;
            audioSource.pitch = 1f;
            audioSource.Play();
        }

        public void PlayStatic(AudioClip staticClip)
        {
            // Solo reproduce si es necesario para evitar cortes
            if (staticClip != null && (audioSource.clip != staticClip || !audioSource.isPlaying))
            {
                audioSource.loop = true;
                audioSource.clip = staticClip;
                audioSource.volume = 1f; 
                audioSource.pitch = 1f;
                audioSource.Play();
            }
        }

        public void Stop()
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }

        public void SetPaused(bool isPaused)
        {
            if (isPaused) audioSource.Pause();
            else audioSource.UnPause();
        }

        // Helpers para las corrutinas del script principal
        public float GetSwitchClipLength()
        {
            return channelSwitchClip != null ? channelSwitchClip.length : 0.1f;
        }

        public float GetCurrentPitch()
        {
            return audioSource.pitch;
        }
    }
}