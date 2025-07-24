namespace TwelveG.AudioController
{
    using System.Collections;
    using TwelveG.UIController;
    using UnityEngine;
    using UnityEngine.Audio;

    public enum AudioGroup
    {
        masterVol,
        musicVol,
        uiVol,
        inGameVol
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Main references")]
        [SerializeField] private AudioMixer masterMixer;

        [Header("Children references")]
        [SerializeField] private AudioPoolsHandler audioPoolsHandler;
        [SerializeField] private AudioFaderHandler audioFaderHandler;

        public AudioPoolsHandler PoolsHandler => audioPoolsHandler;
        public AudioFaderHandler FaderHandler => audioFaderHandler;

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

        public void MainAudioControls(Component sender, object data)
        {
            switch (data)
            {
                case ActivateCanvas cmd:
                    if (cmd.Activate)
                    {
                        SetInGameVol(-80f);
                        SetMusicVol(-80f);
                    }
                    else
                    {
                        SetInGameVol(0f);
                        SetMusicVol(0f);
                    }
                    break;
                default:
                    Debug.LogWarning($"[MainAudioControls] Received unknown command: {data}");
                    break;
            }
        }

        public void FadeAudioGroup(AudioGroup audioGroup, float from, float to, float duration)
        {
            StartCoroutine(FadeAudioCoroutine(audioGroup.ToString(), from, to, duration));
        }

        private IEnumerator FadeAudioCoroutine(string audioGroup, float from, float to, float duration)
        {
            float elapsed = 0f;
            // // Get the channel current volume
            // masterMixer.GetFloat(audioGroup, out _currentVolume);
            // _currentVolume = Mathf.Pow(10, _currentVolume / 20);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newVolume = Mathf.Lerp(from, to, elapsed / duration);
                masterMixer.SetFloat(audioGroup, Mathf.Log10(newVolume) * 20);
                yield return null;
            }

            masterMixer.SetFloat(audioGroup, Mathf.Log10(to) * 20);
        }

        public void SetMasterVoume(float masterVol)
        {
            masterMixer.SetFloat("masterVol", masterVol);
        }

        public void SetMusicVol(float musicVol)
        {
            masterMixer.SetFloat("musicVol", musicVol);
        }

        public void SetUIVol(float uiVol)
        {
            masterMixer.SetFloat("uiVol", uiVol);
        }

        public void SetInGameVol(float inGameVol)
        {
            masterMixer.SetFloat("inGameVol", inGameVol);
        }
    }
}