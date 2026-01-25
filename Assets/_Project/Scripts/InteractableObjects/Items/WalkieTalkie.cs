using System.Collections;
using TMPro;
using TwelveG.DialogsController;
using TwelveG.GameController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class WalkieTalkie : PlayerItemBase, IPlayerItem
    {
        [Header("General References")]
        [SerializeField] private TextMeshProUGUI channelNumberText;
        [SerializeField] private TextMeshProUGUI frequencyText;
        [Space]
        [SerializeField] private WalkieTalkieChannelData[] walkieTalkieChannelData;

        [Space]
        [Header("Modules")]
        [SerializeField] private WalkieTalkieAudioHandler audioHandler;
        [SerializeField] private WalkieTalkieDataHandler dataHandler;
        [SerializeField] private WalkieTalkieCallHandler callHandler;

        [Space]
        [Header("Runtime Data")]
        public WalkieTalkieChannel[] walkieTalkieChannels;

        private WalkieTalkieDataSO currentWalkieTalkieData;

        // Estado
        private bool canSwitchChannel = true;
        private bool characterIsTalking = false;
        private int currentChannelIndex = 0;

        void Start()
        {
            // Auto-asignación de módulos
            if (audioHandler == null) audioHandler = GetComponent<WalkieTalkieAudioHandler>();
            if (dataHandler == null) dataHandler = GetComponent<WalkieTalkieDataHandler>();
            if (callHandler == null) callHandler = GetComponent<WalkieTalkieCallHandler>();

            callHandler.Initialize(audioHandler, () => this.itemIsShown);
        }

        void Update()
        {
            if (isActiveOnGame)
            {
                if (Input.GetKeyDown(KeyCode.K)) ToggleItem();

                if (itemIsShown && canSwitchChannel)
                {
                    if (Input.GetKeyDown(KeyCode.V)) StartCoroutine(SwitchChannel(-1));
                    if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(SwitchChannel(+1));
                }
            }
        }

        public void SetWalkieTalkie(Component sender, object data)
        {
            // 1. Resolver qué SO usar
            WalkieTalkieDataSO targetData = dataHandler.ResolveDataSO(data);

            if (targetData == null)
            {
                Debug.LogWarning($"[WT] Data invalida: {data}");
                return;
            }

            currentWalkieTalkieData = targetData;

            // 2. Construir los canales (Delegado al DataHandler)
            walkieTalkieChannels = dataHandler.BuildChannels(currentWalkieTalkieData);

            // 3. Reproducir estática inicial si corresponde
            PlayCurrentChannelStatic();
        }

        private IEnumerator SwitchChannel(int direction)
        {
            // Fallback de inicialización si es null (Testing)
            if (currentWalkieTalkieData == null)
            {
                currentWalkieTalkieData = dataHandler.GetDefaultData();
                if (currentWalkieTalkieData != null)
                {
                    walkieTalkieChannels = dataHandler.BuildChannels(currentWalkieTalkieData);
                }
                else yield break;
            }

            // Límites
            if (currentChannelIndex == 0 && direction == -1) yield break;
            if (currentChannelIndex == currentWalkieTalkieData.FrequencyData.Count - 1 && direction == +1) yield break;

            // Audio Switch
            audioHandler.PlayChannelSwitch();
            yield return new WaitForFixedUpdate();

            // Cambio de índice
            currentChannelIndex += direction;
            UpdateUI();

            // Audio Estática
            PlayCurrentChannelStatic();

            // Lógica de Diálogos Pendientes
            WalkieTalkieChannel currentChannelObj = walkieTalkieChannels[currentChannelIndex];

            if (currentChannelObj.HasPendingDialog())
            {
                GameEvents.Common.onShowDialog.Raise(this, currentChannelObj.pendingDialog);
                LockControls();
                currentChannelObj.ClearPendingDialog();
                yield break;
            }

            // Lógica de Llamada Entrante
            if (currentChannelIndex == callHandler.MicaChannelIndex && callHandler.IsIncomingCallWaiting)
            {
                callHandler.AcceptWaitingCall();
                LockControls();
            }
        }

        // --- HELPERS VISUALES Y AUDIO ---

        private void UpdateUI()
        {
            frequencyText.text = walkieTalkieChannelData[currentChannelIndex].frequencyText;
            channelNumberText.text = walkieTalkieChannelData[currentChannelIndex].channelText;
        }

        private void PlayCurrentChannelStatic()
        {
            if (currentChannelIndex < walkieTalkieChannels.Length)
            {
                var clip = walkieTalkieChannels[currentChannelIndex].channelClip;
                audioHandler.PlayStatic(clip);
            }
        }

        // --- MÉTODOS PÚBLICOS (CALLBACKS DE LISTENERS) ---

        public void StartDialog(Component sender, object data)
        {
            DialogSO dialog = (DialogSO)data;
            if (dialog == null) return;

            bool shouldShowItem = callHandler.ProcessDialogRequest(dialog, currentChannelIndex);

            if (dialog.characterName == CharacterName.Mica || (dialog.characterName == CharacterName.Simon && !dialog.isSelfDialog))
            {
                LockControls();
            }

            if (shouldShowItem)
            {
                if (currentChannelIndex != callHandler.MicaChannelIndex)
                {
                    StartCoroutine(ReturnToMicaChannelCoroutine());
                }
                if (!itemIsShown) ShowItem();
            }
        }

        public void onLoadDialogForSpecificChannel(Component sender, object data)
        {
            if (data == null) return;
            DialogForChannel dialogInfo = (DialogForChannel)data;

            if (dialogInfo.channelIndex < 0 || dialogInfo.channelIndex >= walkieTalkieChannels.Length) return;

            walkieTalkieChannels[dialogInfo.channelIndex].pendingDialog = dialogInfo.dialogSO;

            if (currentChannelIndex == dialogInfo.channelIndex && itemIsShown)
            {
                GameEvents.Common.onShowDialog.Raise(this, dialogInfo.dialogSO);
                LockControls();
                walkieTalkieChannels[dialogInfo.channelIndex].ClearPendingDialog();
            }
        }

        public void OnDialogEnded(Component sender, object data)
        {
            UnlockControls();
            if (itemIsShown)
            {
                // Solo reproducir estática si no quedó otro diálogo pendiente inmediatamente
                if (!walkieTalkieChannels[currentChannelIndex].HasPendingDialog())
                {
                    PlayCurrentChannelStatic();
                }
            }
        }

        public void ToggleWTAudioSource(Component sender, object data)
        {
            if (data != null && !characterIsTalking) audioHandler.SetPaused((bool)data);
        }

        public void OnDialogNodeRunning(Component sender, object data)
        {
            if (data != null)
            {
                characterIsTalking = (bool)data;
                audioHandler.SetPaused(characterIsTalking);
            }
        }

        public void ToggleItem() => StartCoroutine(ToggleItemCoroutine());

        // --- CORRUTINAS DE ANIMACIÓN ---

        private IEnumerator ToggleItemCoroutine()
        {
            if (anim == null || animationPlaying) yield break;

            if (itemIsShown && canBeToogled) // OCULTAR
            {
                if (currentChannelIndex != callHandler.MicaChannelIndex)
                {
                    yield return StartCoroutine(ReturnToMicaChannelCoroutine());
                }

                animationPlaying = true;
                anim.Play("HideItem");
                itemIsShown = false;
                yield return new WaitUntil(() => !anim.isPlaying);

                GetComponentInParent<PlayerInventory>().HandleTogglingItemsHandState(itemType, false);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;

                if (callHandler.IsIncomingCallWaiting)
                {
                    yield return new WaitForSeconds(5f);
                    callHandler.ResumeRingingIfWaiting();
                }
            }
            else if (!itemIsShown && canBeToogled) // MOSTRAR
            {
                UpdateUI();
                GetComponentInParent<PlayerInventory>().HandleTogglingItemsHandState(itemType, true);
                animationPlaying = true;
                ShowItem();

                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;

                if (!callHandler.IsIncomingCallWaiting)
                {
                    PlayCurrentChannelStatic();
                }
                else
                {
                    if (currentChannelIndex == callHandler.MicaChannelIndex)
                    {
                        callHandler.AcceptWaitingCall();
                        LockControls();
                    }
                }
            }
        }

        private IEnumerator ReturnToMicaChannelCoroutine()
        {
            audioHandler.Stop();
            int targetIndex = callHandler.MicaChannelIndex;

            while (currentChannelIndex != targetIndex)
            {
                int direction = targetIndex > currentChannelIndex ? 1 : -1;
                currentChannelIndex += direction;
                UpdateUI();

                float currentPitch = audioHandler.GetCurrentPitch();
                audioHandler.PlayFastSwitch(currentPitch);

                float waitTime = audioHandler.GetSwitchClipLength() / Mathf.Max(0.01f, currentPitch);
                yield return new WaitForSeconds(waitTime);
            }
        }

        private void LockControls()
        {
            canSwitchChannel = false;
            canBeToogled = false;
        }

        private void UnlockControls()
        {
            canSwitchChannel = true;
            canBeToogled = true;
        }

        public void AllowChannelSwitching(bool allow) => canSwitchChannel = allow;
        public void SetChannelIndex(int index) => currentChannelIndex = index;
    }
}