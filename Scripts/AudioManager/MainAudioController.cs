namespace AudioManager
{
    using System;
    using System.Collections;
    using TwelveG.UIManagement;
    using UnityEngine;
    using UnityEngine.Audio;

    public class MainAudioController : MonoBehaviour
    {
        [SerializeField] private AudioMixer masterMixer;

        public void MainAudioControls(Component sender, object data)
        {
            if ((string)data == "ActivatePauseMenu")
            {
                SetInGameVol(-80f);
                SetMusicVol(-80f);
            }
            else if ((string)data == "DeactivatePauseMenu")
            {
                SetInGameVol(0f);
                SetMusicVol(0f);
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