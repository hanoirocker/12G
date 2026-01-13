using System.Collections;
using TwelveG.SaveSystem;
using TwelveG.UIController;
using UnityEngine;
using UnityEngine.Audio;

namespace TwelveG.AudioController
{
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
        [SerializeField] private AudioZoneHandler audioZoneHandler;
        [SerializeField] private AudioDialogsHandler audioDialogsHandler;

        public AudioPoolsHandler PoolsHandler => audioPoolsHandler;
        public AudioFaderHandler FaderHandler => audioFaderHandler;
        public EnvironmentAudioHandler EnvironmentAudioHandler => audioEnvironmentHandler;
        public AudioZoneHandler AZHandler => audioZoneHandler;
        public AudioDialogsHandler AudioDialogsHandler => audioDialogsHandler;

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

        public void MainAudioControls(Component sender, object data)
        {
            switch (data)
            {
                case ActivateCanvas cmd:
                    if (cmd.Activate)
                    {
                        SetSFXVol(0);
                        SetMusicVol(0);
                    }
                    else
                    {
                        SetSFXVol(0);
                        SetMusicVol(0f);
                    }
                    break;
                default:
                    Debug.LogWarning($"[MainAudioControls] Received unknown command: {data}");
                    break;
            }
        }

        public float GetCurrentChannelVolume(string channel)
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

        // Llamado cuando se recibe onResetWeatherConfigs (no implementado en ningun evento aun)
        public void ResetWeatherConfigs()
        {
            PoolsHandler.ResetWeatherSources();
            AZHandler.ResetAcousticSources();
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

        public void ResetVolumesToInitialValues()
        {
            SetMasterVol(initialMasterVol);
            SetMusicVol(initialMusicVol);
            SetInterfaceVol(initialInterfaceVol);
            SetSFXVol(initialSFXVol);

            masterMixer.SetFloat("inGameLowPassCutOff", 22000f); // Asegura que el LowPass del canal inGameVol esté desactivado
        }

        public void PauseGame(Component sender, object data)
        {
            if (data != null)
            {
                PoolsHandler.PauseActiveAudioSources((bool)data);
            }
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

        public void EnableLowPassOnAmbientChannel(bool enable)
        {
            if (enable)
            {
                masterMixer.SetFloat("ambientLowPassCutOff", 5000f);
            }
            else
            {
                masterMixer.SetFloat("ambientLowPassCutOff", 22000f);
            }
        }

        public IEnumerator LowPassCorutine(string param, float targetValue, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newValue = Mathf.Lerp(22000f, targetValue, elapsed / duration);
                masterMixer.SetFloat(param, newValue);
                yield return null;
            }
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

            // Si se guardó antes, cargar los valores guardados.
            initialMasterVol = data.masterVolume;
            initialInterfaceVol = data.interfaceVolume;
            initialMusicVol = data.musicVolume;
            initialSFXVol = data.sfxVolume;

            // Iniciar Valores
            ResetVolumesToInitialValues();
        }

        public void SaveData(ref GameData data)
        {
            data.masterVolume = GetCurrentChannelVolume("masterVol");
            initialMasterVol = data.masterVolume;
            data.musicVolume = GetCurrentChannelVolume("musicVol");
            initialMusicVol = data.musicVolume;
            data.interfaceVolume = GetCurrentChannelVolume("uiVol");
            initialInterfaceVol = data.interfaceVolume;
            data.sfxVolume = GetCurrentChannelVolume("inGameVol");
            initialSFXVol = data.sfxVolume;
        }
    }
}