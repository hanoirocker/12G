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

        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onShowIncomingCallPanel;
        [SerializeField] private GameEventSO onShowDialog;

        private DialogSO lastDialogReceived = null;
        private WalkieTalkieDataSO currentWalkieTalkieData;
        private AudioSource audioSource;
        private bool canSwitchChannel = true;
        private bool incomingCallWaiting = false;
        private int currentChannelIndex = 0;
        private int micaChannelIndex = 2;
        private int currentDataIndex = 0;

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
            // Test data!
            if (currentWalkieTalkieData == null)
            {
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

            if (currentChannelIndex == micaChannelIndex && incomingCallWaiting)
            {
                incomingCallWaiting = false;
                onShowDialog.Raise(this, lastDialogReceived);
                AllowChannelSwitching(false);
                AllowItemToBeToggled(false);
            }

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
                // Por defecto se vuelve al canal de Mica al ocultar el WT
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

                // Si guardamos el WT desde un canal que no era el de Mica, debemos
                // mostrar el aviso de la llamada entrante.
                if (incomingCallWaiting)
                {
                    // Tiempo de gracia para que en caso de que hayamos guardado el WT desde un canal
                    // que no sea el de Mica y haya habido una llamada de ella entrante,
                    // se muestre el cartel de aceptar llamada unos segundos luego de haberlo guardado.
                    yield return new WaitForSeconds(5f);
                    StartCoroutine(IncomingDialogAlertCourutine());
                }
            }
            else if (!itemIsShown && canBeToogled)
            {
                animationPlaying = true;
                anim.Play("ShowItem");

                // Este evento sirve en particular para indicar que se comenzo a mostrar item, no que se actualizo
                // su estado a 'mostrado'. Escucha por ejemplo Item Canvas para ocultar el texto de alerta.
                onShowingItem.Raise(this, null);
                itemIsShown = true;

                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;

                // Si habia una llamada en espera, se resetea el estado y se informa 
                // DialogManager que comience.
                if (incomingCallWaiting)
                {
                    onShowDialog.Raise(this, lastDialogReceived);
                    incomingCallWaiting = false;
                    AllowChannelSwitching(false);
                    AllowItemToBeToggled(false);
                }
            }
            else
            {
                yield return null;
            }
        }

        public void StartDialog(Component sender, object data)
        {
            lastDialogReceived = (DialogSO)data;

            if (lastDialogReceived == null)
            {
                return;
            }

            if (lastDialogReceived.characterName == CharacterName.Mica)
            {
                if (itemIsShown && currentChannelIndex == micaChannelIndex)
                {
                    AllowItemToBeToggled(false);
                    AllowChannelSwitching(false);
                    onShowDialog.Raise(this, lastDialogReceived);
                }
                // Si Mica empieza un dialogo con Simon pero no estamos en en canal de ella ..
                else if (itemIsShown && currentChannelIndex != micaChannelIndex)
                {
                    incomingCallWaiting = true;
                }
                // Siempre que el jugador guarda el WT se vuelve al canal de Mica, asi que alcanza
                // con chekear el estado del item para mostrar la alerta.
                else if (!itemIsShown)
                {
                    incomingCallWaiting = true;
                    StartCoroutine(IncomingDialogAlertCourutine());
                }
            }

            // Si Simon debe comenzar un dialogo con Mica
            if (lastDialogReceived.characterName == CharacterName.Simon && !lastDialogReceived.isSelfDialog)
            {
                // Si no está en el canal de Mica, va hasta el mismo y comienza el dialogo.
                if (currentChannelIndex != micaChannelIndex)
                {
                    // TODO: Agrega dialogo tipo 'Debo hablar con Mica' a ejecutar mientras se cambia
                    // de canal.
                    StartCoroutine(ReturnToMicaChannelCoroutine());
                }

                AllowItemToBeToggled(false);
                if (!itemIsShown) ShowItem();
                AllowChannelSwitching(false);
                onShowDialog.Raise(this, lastDialogReceived);
            }
            else if (lastDialogReceived.characterName == CharacterName.Simon && lastDialogReceived.isSelfDialog)
            {
                AllowChannelSwitching(false);
                onShowDialog.Raise(this, lastDialogReceived);
            }
        }

        private IEnumerator IncomingDialogAlertCourutine()
        {
            // Empieza a sonar avisando que hay una llamada de Mica y avisa al Item Canvas
            // que muestre texto 'Accept Call [K]'
            onShowIncomingCallPanel.Raise(this, true);
            audioSource.clip = incomingDialogAlertClip;
            audioSource.Play();

            yield return new WaitUntil(() => itemIsShown);

            // Dejar de sonar y ocultar el panel del item canvas
            onShowIncomingCallPanel.Raise(this, false);
            AllowItemToBeToggled(false);
            audioSource.Stop();
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

            if (currentWalkieTalkieData == null)
            {
                Debug.Log($"[WT] Null channels data for this event!");
                return;
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