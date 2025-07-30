namespace TwelveG.AudioController
{
    using TwelveG.SaveSystem;
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

    public class AudioManager : MonoBehaviour, IDataPersistence
    {
        public static AudioManager Instance;

        [Header("Main Settings")]
        [SerializeField, Range(0.1f, 1f)] private float defaultValue = 0.5f; // For all channels

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

        private float initialMasterVol;
        private float initialMusicVol;
        private float initialSFXVol;
        private float initialInterfaceVol;

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
            SetMasterVol(initialMasterVol);
            SetMusicVol(initialMusicVol);
            SetInterfaceVol(initialInterfaceVol);
            SetSFXVol(initialSFXVol);
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

        private float GetCurrentChannelVolume(string channel)
        {
            float currentValue;
            if (masterMixer.GetFloat(channel, out currentValue))
            {
                return AudioUtils.DecibelsToNormalized(currentValue);
            }
            else
            {
                Debug.LogError($"[AudioManager] GetCurrentChannelVolume couldn't recognize channel {channel}");
                return defaultValue; // default volume for every channel
            }
        }

        public float GetInitialChannelVolume(AudioGroup audioGroup)
        {
            switch (audioGroup)
            {
                case AudioGroup.masterVol:
                    return initialMasterVol;
                case AudioGroup.musicVol:
                    return initialMusicVol;
                case AudioGroup.uiVol:
                    return initialInterfaceVol;
                case AudioGroup.inGameVol:
                    return initialSFXVol;
                default:
                    Debug.LogError($"[AudioManager] GetInitialChannelVolume didn't recognize audioGroup {audioGroup.ToString()}");
                    break;
            }
            Debug.LogWarning($"[AudioManager] GetInitialChannelVolume: returning defaultValue");
            return defaultValue;
        }

        public void SetMasterVol(float masterVol)
        {
            masterMixer.SetFloat("masterVol", AudioUtils.NormalizedToDecibels(masterVol));
        }

        public void SetMusicVol(float musicVol)
        {
            masterMixer.SetFloat("musicVol", AudioUtils.NormalizedToDecibels(musicVol));
        }

        public void SetInterfaceVol(float uiVol)
        {
            masterMixer.SetFloat("uiVol", AudioUtils.NormalizedToDecibels(uiVol));
        }

        public void SetSFXVol(float inGameVol)
        {
            masterMixer.SetFloat("inGameVol", AudioUtils.NormalizedToDecibels(inGameVol));
        }

        public void LoadData(GameData data)
        {
            // Si nunca se guardó, debe iniciar con los valores por default
            // No los setearemos en la estructura GameData sino en cada Manager.
            if (data.savesNumber == 0)
            {
                initialMasterVol = defaultValue;
                initialInterfaceVol = defaultValue;
                initialMusicVol = defaultValue;
                initialSFXVol = defaultValue;

                return;
            }

            // Si se guardó mas de una vez, cargar los valores guardados.
            initialMasterVol = data.masterVolume;
            initialInterfaceVol = data.interfaceVolume;
            initialMusicVol = data.musicVolume;
            initialSFXVol = data.sfxVolume;
        }

        public void SaveData(ref GameData data)
        {
            data.masterVolume = GetCurrentChannelVolume("masterVol");
            data.musicVolume = GetCurrentChannelVolume("musicVol"); ;
            data.interfaceVolume = GetCurrentChannelVolume("uiVol"); ;
            data.sfxVolume = GetCurrentChannelVolume("inGameVol"); ;
        }
    }
}