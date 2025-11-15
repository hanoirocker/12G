using System.Collections;
using TwelveG.DialogsController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class WalkieTalkie : PlayerItemBase, IPlayerItem
    {
        [Header("Data SO References")]
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieEveningData;
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieNightData;

        [Header("Audio")]
        [SerializeField] private AudioClip channelSwitchAudioClip;
        [SerializeField] private AudioClip incomingDialogAlertClip;
        [SerializeField, Range(0f, 1f)] private float switchChannelVolume = 0.8f;

        private WalkieTalkieDataSO currentWalkieTalkieData;
        private bool canSwitchChannel = true;
        private int currentChannelIndex = 0;
        private int micaChannelIndex = 2;
        private int currentDataIndex = 0;
        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleItem();
            }
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
            // Checks
            if (currentWalkieTalkieData == null)
            {
                // Test data
                currentWalkieTalkieData = walkieTalkieEveningData[0];
            }
            if (currentChannelIndex == 0 && direction == -1)
            {
                yield break;
            }
            if (currentChannelIndex == currentWalkieTalkieData.FrequencyData.Count - 1 && direction == +1)
            {
                yield break;
            }

            currentChannelIndex += direction;

            Debug.Log($"Canal actual: {currentChannelIndex}");

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

        private IEnumerator ReturnToMicaChannelCoroutine()
        {
            audioSource.Stop();
            audioSource.clip = null;

            while (currentChannelIndex != micaChannelIndex)
            {
                int direction = micaChannelIndex > currentChannelIndex ? 1 : -1;
                currentChannelIndex += direction;

                audioSource.PlayOneShot(channelSwitchAudioClip, switchChannelVolume);
                float waitTime = channelSwitchAudioClip.length / Mathf.Max(0.01f, audioSource.pitch);
                yield return new WaitForSeconds(waitTime);
            }
        }

        private IEnumerator ToggleItemCoroutine()
        {
            if (anim == null)
                yield break;

            if (animationPlaying)
                yield break;

            if (itemIsShown && canBeToogled)
            {
                if (currentChannelIndex != micaChannelIndex)
                {
                    yield return StartCoroutine(ReturnToMicaChannelCoroutine());
                }

                animationPlaying = true;
                // Si está visible, ejecuta animación para ocultar
                anim.Play("HideItem");
                itemIsShown = false;
                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;
            }
            else if (!itemIsShown && canBeToogled)
            {
                animationPlaying = true;
                // Si está oculto, ejecuta animación para mostrar
                anim.Play("ShowItem");
                // Este evento sirve en particular para indicar que se comenzo a mostrar item, no que se actualizo
                // su estado a 'mostrado'. Escucha por ejemplo Item Canvas para ocultar el texto de alerta.
                onShowingItem.Raise(this, null);
                itemIsShown = true;
                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;
            }
            else
            {
                yield return null;
            }
        }

        public void StartDialog(Component sender, object data)
        {
            var DialogSOData = (DialogSO)data;

            if (DialogSOData == null)
            {
                Debug.LogWarning("No DialogSOData but starDialog received");
                return;
            }

            // Si el dialogo se dirige a Mica, setea el canal al de Mica y bloquea el cambio de canal, ademas de mostrar el item
            if (DialogSOData.characterName == CharacterName.Simon && !DialogSOData.isSelfDialog)
            {
                currentChannelIndex = 2;

                AllowItemToBeToggled(false);
                ShowItem();
                AllowChannelSwitching(false);
            }
            if (DialogSOData.characterName == CharacterName.Mica && !itemIsShown)
            {
                StartCoroutine(IncomingDialogAlertCourutine());
            }
        }

        private IEnumerator IncomingDialogAlertCourutine()
        {
            audioSource.loop = true;
            audioSource.clip = incomingDialogAlertClip;
            audioSource.Play();

            yield return new WaitUntil(() => itemIsShown);

            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
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

        public void ToggleItem()
        {
            StartCoroutine(ToggleItemCoroutine());
        }
    }
}