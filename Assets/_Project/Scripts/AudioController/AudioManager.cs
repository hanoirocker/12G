namespace TwelveG.AudioController
{
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
        [SerializeField] private EnvironmentAudioHandler audioEnvironmentHandler;
        [SerializeField] private AudioRainZoneHandler audioRainHandler;

        public AudioPoolsHandler PoolsHandler => audioPoolsHandler;
        public AudioFaderHandler FaderHandler => audioFaderHandler;
        public EnvironmentAudioHandler EnvironmentHandler => audioEnvironmentHandler;
        public AudioRainZoneHandler RainHandler => audioRainHandler;

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
                        SetSFXVol(-80f);
                        SetMusicVol(-80f);
                    }
                    else
                    {
                        SetSFXVol(0f);
                        SetMusicVol(0f);
                    }
                    break;
                default:
                    Debug.LogWarning($"[MainAudioControls] Received unknown command: {data}");
                    break;
            }
        }

        public void SetMasterVol(float masterVol)
        {
            masterMixer.SetFloat("masterVol", masterVol);
        }

        public void SetMusicVol(float musicVol)
        {
            masterMixer.SetFloat("musicVol", musicVol);
        }

        public void SetInterfaceVol(float uiVol)
        {
            masterMixer.SetFloat("uiVol", uiVol);
        }

        public void SetSFXVol(float inGameVol)
        {
            masterMixer.SetFloat("inGameVol", inGameVol);
        }
    }
}