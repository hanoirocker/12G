using System;
using System.Collections;
using TMPro;
using TwelveG.DialogsController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    [System.Serializable]
    public class WalkieTalkieChannel
    {
        public int channelIndex;
        public AudioClip channelClip;
        public DialogSO pendingDialog;

        public bool HasPendingDialog()
        {
            return pendingDialog != null;
        }

        public void ClearPendingDialog()
        {
            pendingDialog = null;
        }
    }

    [System.Serializable]
    public struct WalkieTalkieChannelData
    {
        public string channelText;
        public string frequencyText;
    }

    public class WalkieTalkie : PlayerItemBase, IPlayerItem
    {
        [Header("General References")]
        [SerializeField] private TextMeshProUGUI channelNumberText;
        [SerializeField] private TextMeshProUGUI frequencyText;
        [Space]
        [SerializeField] private WalkieTalkieChannelData[] walkieTalkieChannelData;

        [Space]
        [Header("Data SO References")]
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieEveningData;
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieNightData;
        [SerializeField] private WalkieTalkieDataSO walkieTalkieFreeRoamData;

        [Space]
        [Header("Audio")]
        [SerializeField] private AudioClip channelSwitchAudioClip;
        [SerializeField] private AudioClip incomingDialogAlertClip;
        [SerializeField, Range(0f, 1f)] private float alertClipVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float switchChannelVolume = 0.8f;

        [Space]
        [Header("Runtime Data")]
        public WalkieTalkieChannel[] walkieTalkieChannels;

        private DialogSO lastDialogReceived = null;
        private WalkieTalkieDataSO currentWalkieTalkieData;
        private AudioSource audioSource;
        private bool canSwitchChannel = true;
        private bool characterIsTalking = false;
        private bool incomingCallWaiting = false;
        private int currentChannelIndex = 0;
        private int micaChannelIndex = 2; // Canal 3

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (isActiveOnGame)
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
        }

        private IEnumerator SwitchChannel(int direction)
        {
            // Validaciones de seguridad
            if (currentWalkieTalkieData == null)
            {
                // Fallback por si no se inicializó (testing)
                if (walkieTalkieEveningData != null && walkieTalkieEveningData.Length > 0)
                    currentWalkieTalkieData = walkieTalkieEveningData[0];
                else
                    yield break;
            }

            // Límites del dial
            if (currentChannelIndex == 0 && direction == -1) yield break;
            if (currentChannelIndex == currentWalkieTalkieData.FrequencyData.Count - 1 && direction == +1) yield break;

            // Reproducir sonido de cambio de canal
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.PlayOneShot(channelSwitchAudioClip);
            yield return new WaitForFixedUpdate();

            currentChannelIndex += direction;
            WalkieTalkieChannel currentChannelObj = walkieTalkieChannels[currentChannelIndex];

            frequencyText.text = walkieTalkieChannelData[currentChannelIndex].frequencyText;
            channelNumberText.text = walkieTalkieChannelData[currentChannelIndex].channelText;

            try
            {
                if (currentChannelObj.channelClip != null)
                {
                    audioSource.clip = currentChannelObj.channelClip;
                    audioSource.Play();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[WT] Error reproduciendo audio canal: {e.Message}");
            }

            if (currentChannelObj.HasPendingDialog())
            {
                Debug.Log($"[WT] Disparando diálogo agendado en canal {currentChannelIndex}");

                GameEvents.Common.onShowDialog.Raise(this, currentChannelObj.pendingDialog);

                AllowChannelSwitching(false);
                AllowItemToBeToggled(false);

                // Limpiamos el diálogo para que no se repita loop
                currentChannelObj.ClearPendingDialog();
                yield break;
            }

            if (currentChannelIndex == micaChannelIndex && incomingCallWaiting)
            {
                incomingCallWaiting = false;

                GameEvents.Common.onShowDialog.Raise(this, lastDialogReceived);

                AllowChannelSwitching(false);
                AllowItemToBeToggled(false);
                yield break;
            }
        }

        public void SetWalkieTalkie(Component sender, object data)
        {
            WalkieTalkieDataSO targetData = ResolveDataSO(data);

            if (targetData == null)
            {
                Debug.LogWarning($"[WT] No se pudo resolver la data para el input: {data}");
                return;
            }

            currentWalkieTalkieData = targetData;

            // Construir / Reconstruir los canales con la data obtenida
            InitializeChannels(currentWalkieTalkieData);
        }

        // Helper para encontrar el SO correcto según el tipo de dato recibido
        private WalkieTalkieDataSO ResolveDataSO(object data)
        {
            if (data is string strData && strData == "FreeRoam")
            {
                return walkieTalkieFreeRoamData;
            }

            if (data is EventContextData context)
            {
                if (context.sceneEnum == SceneEnum.Evening)
                {
                    return FindDataByEvent(walkieTalkieEveningData, context.eventEnum);
                }
                else if (context.sceneEnum == SceneEnum.Night)
                {
                    return FindDataByEvent(walkieTalkieNightData, context.eventEnum);
                }
            }

            return null;
        }

        // Helper para buscar en los arrays (evita repetir el bucle for)
        private WalkieTalkieDataSO FindDataByEvent(WalkieTalkieDataSO[] dataArray, EventsEnum targetEvent)
        {
            if (dataArray == null) return null;

            for (int i = 0; i < dataArray.Length; i++)
            {
                if (dataArray[i].eventName == targetEvent)
                {
                    return dataArray[i]; // Retornamos el SO encontrado
                }
            }
            return null;
        }

        // Helper para inicializar el array
        private void InitializeChannels(WalkieTalkieDataSO data)
        {
            int frequencyCount = data.FrequencyData.Count;

            if (walkieTalkieChannels == null || walkieTalkieChannels.Length != frequencyCount)
            {
                walkieTalkieChannels = new WalkieTalkieChannel[frequencyCount];
            }

            for (int i = 0; i < frequencyCount; i++)
            {
                if (walkieTalkieChannels[i] == null)
                    walkieTalkieChannels[i] = new WalkieTalkieChannel();

                walkieTalkieChannels[i].channelIndex = i;

                if (data.FrequencyData[i].clips.Count > 0)
                    walkieTalkieChannels[i].channelClip = data.FrequencyData[i].clips[0];
                else
                    walkieTalkieChannels[i].channelClip = null;

                walkieTalkieChannels[i].ClearPendingDialog();
            }

            if (currentChannelIndex < walkieTalkieChannels.Length)
            {
                var initialClip = walkieTalkieChannels[currentChannelIndex].channelClip;
                if (initialClip != null)
                {
                    audioSource.clip = initialClip;
                }
            }
        }

        // Callback del GameEvent: "onLoadDialogForSpecificChannel"
        // Inyecta el diálogo en el canal correspondiente
        public void onLoadDialogForSpecificChannel(Component sender, object data)
        {
            if (data == null) return;

            DialogForChannel dialogInfo = (DialogForChannel)data;

            if (dialogInfo.channelIndex < 0 || dialogInfo.channelIndex >= walkieTalkieChannels.Length)
            {
                Debug.LogWarning("[WT] Invalid channel index received for specific dialog.");
                return;
            }

            walkieTalkieChannels[dialogInfo.channelIndex].pendingDialog = dialogInfo.dialogSO;

            if (currentChannelIndex == dialogInfo.channelIndex && itemIsShown)
            {
                GameEvents.Common.onShowDialog.Raise(this, dialogInfo.dialogSO);
                AllowChannelSwitching(false);
                AllowItemToBeToggled(false);
                walkieTalkieChannels[dialogInfo.channelIndex].ClearPendingDialog();
            }
        }

        // Al terminar diálogo, restaurar el ruido de fondo si corresponde
        public void OnDialogEnded(Component sender, object data)
        {
            AllowChannelSwitching(true);
            AllowItemToBeToggled(true);

            if (itemIsShown)
            {
                // Verificamos que el canal exista y tenga clip
                if (currentChannelIndex < walkieTalkieChannels.Length)
                {
                    var currentChannelObj = walkieTalkieChannels[currentChannelIndex];

                    // Solo reproducimos ruido si NO quedó otro diálogo pendiente (raro, pero posible)
                    if (!currentChannelObj.HasPendingDialog() && currentChannelObj.channelClip != null)
                    {
                        if (audioSource.clip != currentChannelObj.channelClip || !audioSource.isPlaying)
                        {
                            audioSource.clip = currentChannelObj.channelClip;
                            audioSource.Play();
                        }
                    }
                }
            }
        }

        public void StartDialog(Component sender, object data)
        {
            Debug.Log($"WT recibe desde {sender.name} el diálogo: {data}");

            lastDialogReceived = (DialogSO)data;

            if (lastDialogReceived == null) return;

            // Lógica de Micaela llamando
            if (lastDialogReceived.characterName == CharacterName.Mica)
            {
                if (itemIsShown && currentChannelIndex == micaChannelIndex)
                {
                    AllowItemToBeToggled(false);
                    AllowChannelSwitching(false);
                    GameEvents.Common.onShowDialog.Raise(this, lastDialogReceived);
                }
                else if (itemIsShown && currentChannelIndex != micaChannelIndex)
                {
                    incomingCallWaiting = true;
                }
                else if (!itemIsShown)
                {
                    incomingCallWaiting = true;
                    StartCoroutine(IncomingDialogAlertCourutine());
                }
            }

            // Lógica de Simon iniciando (SelfDialog o llamando a Mica)
            if (lastDialogReceived.characterName == CharacterName.Simon)
            {
                if (!lastDialogReceived.isSelfDialog)
                {
                    if (currentChannelIndex != micaChannelIndex)
                    {
                        StartCoroutine(ReturnToMicaChannelCoroutine());
                    }

                    if (!itemIsShown) ShowItem();
                }

                AllowItemToBeToggled(false);
                AllowChannelSwitching(false);
                GameEvents.Common.onShowDialog.Raise(this, lastDialogReceived);
            }

            if (lastDialogReceived.characterName == CharacterName.Unknown)
            {
                incomingCallWaiting = true;
                StartCoroutine(IncomingDialogAlertCourutine());
            }
        }

        private IEnumerator IncomingDialogAlertCourutine()
        {
            GameEvents.Common.onShowIncomingCallPanel.Raise(this, true);
            audioSource.clip = incomingDialogAlertClip;
            audioSource.volume = alertClipVolume;
            audioSource.Play();

            yield return new WaitUntil(() => itemIsShown);

            GameEvents.Common.onShowIncomingCallPanel.Raise(this, false);
            AllowItemToBeToggled(false);
            audioSource.Stop();
            audioSource.clip = null;
        }

        private IEnumerator ReturnToMicaChannelCoroutine()
        {
            audioSource.Stop();
            audioSource.clip = null;

            while (currentChannelIndex != micaChannelIndex)
            {
                int direction = micaChannelIndex > currentChannelIndex ? 1 : -1;
                currentChannelIndex += direction;

                frequencyText.text = walkieTalkieChannelData[currentChannelIndex].frequencyText;
                channelNumberText.text = walkieTalkieChannelData[currentChannelIndex].channelText;

                audioSource.PlayOneShot(channelSwitchAudioClip, switchChannelVolume);
                float waitTime = channelSwitchAudioClip.length / Mathf.Max(0.01f, audioSource.pitch);
                yield return new WaitForSeconds(waitTime);
            }
        }

        private IEnumerator ToggleItemCoroutine()
        {
            if (anim == null || animationPlaying) yield break;

            if (itemIsShown && canBeToogled)
            {
                // Volver al canal de Mica si no estamos ahí
                if (currentChannelIndex != micaChannelIndex)
                {
                    yield return StartCoroutine(ReturnToMicaChannelCoroutine());
                }

                animationPlaying = true;
                anim.Play("HideItem");
                itemIsShown = false;
                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;

                // Lógica de llamada perdida durante el guardado
                if (incomingCallWaiting)
                {
                    yield return new WaitForSeconds(5f);
                    StartCoroutine(IncomingDialogAlertCourutine());
                }
            }
            else if (!itemIsShown && canBeToogled)
            {
                frequencyText.text = walkieTalkieChannelData[currentChannelIndex].frequencyText;
                channelNumberText.text = walkieTalkieChannelData[currentChannelIndex].channelText;

                animationPlaying = true;
                ShowItem();

                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;

                // Al sacar el WT, si no hay llamada entrante, reproducir el ruido del canal actual
                if (!incomingCallWaiting)
                {
                    // Revisamos si el canal actual tiene ruido o diálogo pendiente
                    WalkieTalkieChannel currentCh = walkieTalkieChannels[currentChannelIndex];
                    if (!currentCh.HasPendingDialog() && currentCh.channelClip != null)
                    {
                        audioSource.clip = currentCh.channelClip;
                        audioSource.Play();
                    }
                }

                if (incomingCallWaiting)
                {
                    GameEvents.Common.onShowDialog.Raise(this, lastDialogReceived);
                    incomingCallWaiting = false;
                    AllowChannelSwitching(false);
                    AllowItemToBeToggled(false);
                }
            }
        }

        public void AllowChannelSwitching(bool allow)
        {
            canSwitchChannel = allow;
        }

        public void SetChannelIndex(int index)
        {
            currentChannelIndex = index;
        }

        // Recibe "onPauseGame"
        public void ToggleWTAudioSource(Component sender, object data)
        {
            if (data != null && !characterIsTalking)
            {
                if ((bool)data)
                {
                    audioSource.Pause();
                }
                else
                {
                    audioSource.UnPause();
                }
            }
        }

        public void OnDialogNodeRunning(Component sender, object data)
        {
            if (data != null)
            {
                characterIsTalking = (bool)data;

                if ((bool)data)
                {
                    audioSource.Pause();
                }
                else
                {
                    audioSource.UnPause();
                }
            }
        }

        public void ToggleItem()
        {
            StartCoroutine(ToggleItemCoroutine());
        }
    }
}