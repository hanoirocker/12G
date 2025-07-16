namespace TwelveG.AudioManager
{
    using TwelveG.UIManagement;
    using UnityEngine;
    using UnityEngine.Audio;

    public class MainAudioController : MonoBehaviour
    {
        [SerializeField] private AudioMixer masterMixer;

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