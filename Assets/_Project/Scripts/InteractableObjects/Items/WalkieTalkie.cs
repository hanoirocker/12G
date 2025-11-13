using System.Collections;
using TwelveG.AudioController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class WalkieTalkie : PlayerItemBase
    {
        [Header("Data SO References")]
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieEveningData;
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieNightData;

        [Header("Audio")]
        [SerializeField] private AudioClip channelSwitchAudioClip;
        [SerializeField, Range(0f, 1f)] private float switchChannelVolume = 0.8f;

        private WalkieTalkieDataSO currentWalkieTalkieData;
        private bool canSwitchChannel = true;
        private int currentChannelIndex = 0;
        private int currentDataIndex = 0;
        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (itemIsShown && canSwitchChannel)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    StartCoroutine(SwitchChannel(-1));
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    StartCoroutine(SwitchChannel(+1));
                }
            }
        }

        // Lógica para cambiar de canal en el walkie-talkie
        private IEnumerator SwitchChannel(int direction)
        {
            if (currentChannelIndex == 0 && direction == -1)
            {
                yield break;
            }
            if (currentChannelIndex == currentWalkieTalkieData.FrequencyData.Count - 1 && direction == +1)
            {
                yield break;
            }

            currentChannelIndex += direction;

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            yield return new WaitForFixedUpdate();
            // Reproduce el sonido de cambio de canal
            audioSource.PlayOneShot(channelSwitchAudioClip);

            // Cambia el clip de audio al del nuevo canal
            try
            {
                audioSource.clip = currentWalkieTalkieData.FrequencyData[currentChannelIndex].clips[0];
            }
            catch (System.Exception)
            {
                Debug.LogWarning("El canal seleccionado no tiene audio asignado.");
                yield break;
            }

            audioSource.Play();
        }

        public void AllowChannelSwitching(bool allow)
        {
            canSwitchChannel = allow;
        }

        public void SetWalkieTalkie(Component sender, object data)
        {
            var gameContext = (EventContextData)data;
            SceneEnum sceneEnum = gameContext.sceneEnum;
            EventsEnum eventEnum = gameContext.eventEnum;

            if (sceneEnum == SceneEnum.Evening && walkieTalkieEveningData != null && walkieTalkieEveningData.Length > 0)
            {
                for (int i = 0; i < walkieTalkieEveningData.Length; i++)
                {
                    if (walkieTalkieEveningData[i].eventName == eventEnum)
                    {
                        currentDataIndex = i;
                        break;
                    }
                }
                currentWalkieTalkieData = walkieTalkieEveningData[currentDataIndex];
            }
            else if (sceneEnum == SceneEnum.Night && walkieTalkieNightData != null && walkieTalkieNightData.Length > 0)
            {
                for (int i = 0; i < walkieTalkieNightData.Length; i++)
                {
                    if (walkieTalkieNightData[i].eventName == eventEnum)
                    {
                        currentDataIndex = i;
                        break;
                    }
                }
                currentWalkieTalkieData = walkieTalkieNightData[currentDataIndex];
            }

            audioSource.clip = currentWalkieTalkieData.FrequencyData[currentChannelIndex].clips[0];
        }

        public void PauseAudioSource()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }

        public void SetChannelIndex(int index)
        {
            currentChannelIndex = index;
        }
    }
}