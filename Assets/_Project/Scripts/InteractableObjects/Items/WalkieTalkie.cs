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

        private int channelCount = 0;

        private Coroutine randomSwitchingCR;

        void Start()
        {
            // Auto-asignación de módulos
            if (audioHandler == null) audioHandler = GetComponent<WalkieTalkieAudioHandler>();
            if (dataHandler == null) dataHandler = GetComponent<WalkieTalkieDataHandler>();
            if (callHandler == null) callHandler = GetComponent<WalkieTalkieCallHandler>();

            callHandler.Initialize(audioHandler, () => this.itemIsShown);

            currentChannelIndex = callHandler.MicaChannelIndex;
        }

        void Update()
        {
            if (isActiveOnGame)
            {
                if (Input.GetKeyDown(KeyCode.K)) ToggleItem();

                if (itemIsShown && canSwitchChannel)
                {
                    if (Input.GetKeyDown(KeyCode.V)) StartCoroutine(SwitchChannel(currentChannelIndex - 1, true));
                    if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(SwitchChannel(currentChannelIndex + 1, true));

                    // for debug
                    // -1 channel with no switching sound
                    // if (Input.GetKeyDown(KeyCode.N)) StartCoroutine(SwitchChannel(currentChannelIndex - 1, false)); 
                    // +1 channel with no switching sound
                    // if (Input.GetKeyDown(KeyCode.M)) StartCoroutine(SwitchChannel(currentChannelIndex + 1, false));
                    // start random channel switching 
                    // if (Input.GetKeyDown(KeyCode.O)) StartRandomChannelSwitching();
                    // stop random channel switching
                    // if (Input.GetKeyDown(KeyCode.P)) StopRandomChannelSwitching();
                }
            }
        }

        public void SetWalkieTalkie(Component sender, object data)
        {
            // Resolver qué SO usar (Delegado al DataHandler)
            WalkieTalkieDataSO targetData = dataHandler.ResolveDataSO(data);

            if (targetData == null)
            {
                Debug.LogWarning($"[WT] Data invalida: {data}");
                return;
            }

            currentWalkieTalkieData = targetData;

            // Logica de inicialización
            walkieTalkieChannels = dataHandler.BuildChannels(currentWalkieTalkieData);

            channelCount = walkieTalkieChannels.Length;

            currentChannelIndex = callHandler.MicaChannelIndex;
            UpdateUI();

            // Reproducir estática si corresponde
            PlayCurrentChannelStatic();
        }

        private IEnumerator SwitchChannel(int newChannel, bool manual)
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
            if (newChannel > channelCount -1 || newChannel < 0) yield break;    
            
            if (newChannel > currentWalkieTalkieData.FrequencyData.Count - 1) yield break;
        
            currentChannelIndex = newChannel;

            if (manual) 
            {
                // Audio Switch
                LockControls();
                yield return StartCoroutine(audioHandler.PlayChannelSwitch());
                UnlockControls();
            }
            else
            {
                // Stop static
                audioHandler.StopAudioSource();
            }

            UpdateUI();

            // Audio Estática
            PlayCurrentChannelStatic();

            // Lógica de Diálogos Pendientes
            WalkieTalkieChannel currentChannelObj = walkieTalkieChannels[currentChannelIndex];

            // 1. CHEQUEO DE LORE (Prioridad 1)
            if (currentChannelObj.loreClip != null && !currentChannelObj.hasPlayedLore)
            {
                //LockControls();

                // Reproducimos el Lore clips si existen
                audioHandler.PlayLoreClip(currentChannelObj.loreClip);

                // Esperamos que termine el audio
                yield return new WaitForSeconds(currentChannelObj.loreClip.length);

                // Marcamos como escuchado para que si vuelve al canal, suene estática
                currentChannelObj.hasPlayedLore = true;

                // ¿Hay reacción de Simon?
                if (currentChannelObj.reactionDialog != null)
                {
                    GameEvents.Common.onShowDialog.Raise(this, currentChannelObj.reactionDialog);
                }

                // Al terminar el evento especial, pasamos a la estática de ese canal
                audioHandler.PlayStatic(currentChannelObj.staticClip);

                UnlockControls();
            }
            // 2. CHEQUEO DE ESTÁTICA NORMAL (Si no hubo lore o ya pasó)
            else
            {
                // Reproducimos la estática propia del canal
                audioHandler.PlayStatic(currentChannelObj.staticClip);
            }

            // 3. CHEQUEO DE DIÁLOGOS PENDIENTES (Policía / Eventos Scriptados)
            if (currentChannelObj.HasPendingDialog())
            {
                GameEvents.Common.onShowDialog.Raise(this, currentChannelObj.pendingDialog);
                LockControls();
                currentChannelObj.ClearPendingDialog();
                yield break;
            }

            // 4. Lógica de Llamada Entrante (Mica)
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
                var clip = walkieTalkieChannels[currentChannelIndex].staticClip;
                audioHandler.PlayStatic(clip);
            }
        }

        // --- MÉTODOS PÚBLICOS (CALLBACKS DE LISTENERS) ---

        public void StartDialog(Component sender, object data)
        {
            DialogSO dialog = (DialogSO)data;
            if (dialog == null) return;

            // El Manager decide si suena o si se atiende. 
            // Devuelve TRUE solo si es Simon hablando (auto-show).
            bool shouldShowItem = callHandler.ProcessDialogRequest(dialog, currentChannelIndex);

            // Se bloquea el item si estamos atendiendo la llamada ahora mismo.
            bool isMicaCall = dialog.characterName == CharacterName.Mica;
            bool isSimonCall = dialog.characterName == CharacterName.Simon && !dialog.isSelfDialog;

            if (isMicaCall)
            {
                // Solo bloqueamos si lo tengo en la mano Y estoy en su canal
                // Si está guardado, NO BLOQUEAMOS (para poder sacarlo con K)
                // Si está en otro canal, NO BLOQUEAMOS (para poder cambiar de canal con V/B)
                if (itemIsShown && currentChannelIndex == callHandler.MicaChannelIndex)
                {
                    LockControls();
                }
            }
            else if (isSimonCall)
            {
                // Si habla Simon, como forzamos que salga el item, bloqueamos siempre
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
            if (data != null) audioHandler.SetPaused((bool)data);
        }

        public void OnDialogNodeRunning(Component sender, object data)
        {
            if (data != null)
            {
                characterIsTalking = (bool)data;
                // Llama a SetPaused en el AudioHandler
                if (characterIsTalking)
                {
                    audioHandler.SetPaused(true);
                }
                else
                {
                    if (!walkieTalkieChannels[currentChannelIndex].HasPendingDialog())
                    {
                        PlayCurrentChannelStatic();
                    }
                }
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


        public void SwitchToRandomChannel()
        {
            int newChannel;
            // make sure the new random channel is not the same, to avoid affecting the perceived delays
            do
            {
                newChannel = UnityEngine.Random.Range(0, channelCount);
            } while (newChannel == currentChannelIndex);
            
            StartCoroutine(SwitchChannel(newChannel, false));
        }

        public void StartRandomChannelSwitching()
        {
            float minDelay = 0.15f; // hardcoded here for now
            float maxDelay = 1.0f;
            // stop any preexisting coroutines to make sure there's only one active at a time
            if (randomSwitchingCR != null) StopCoroutine(randomSwitchingCR);
            // keep track of the coroutine to be able to stop it later
            randomSwitchingCR = StartCoroutine(LoopRandomChannelSwitch(minDelay, maxDelay));
        }

        private IEnumerator LoopRandomChannelSwitch(float minDelay, float maxDelay)
            {
                while (true)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay));
                    SwitchToRandomChannel();
                }
            }

        public void StopRandomChannelSwitching()
        {
            if (randomSwitchingCR != null) StopCoroutine(randomSwitchingCR);
        }
    }
}