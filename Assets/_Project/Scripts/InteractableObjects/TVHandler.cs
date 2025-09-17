namespace TwelveG.PlayerController
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using UnityEngine;
    using UnityEngine.Video;

    public class TVHandler : MonoBehaviour
    {
        [System.Serializable]
        public class TVChannel
        {
            public string channelName;
            public VideoClip videoClip;
            public RenderTexture renderTexture;
            public bool isNewsChannel;
            [HideInInspector] public double lastPlaybackTime;
            [HideInInspector] public bool isPrepared;
        }

        [Header("Settings And References")]
        public int initialChannelIndex = 0;
         [SerializeField, Range(0.3f, 0.8f)] private float maxTvVolume = 0.5f;
        public List<TVChannel> channels;

        [SerializeField] EventsControlCanvasInteractionTextSO interactWithTVSO;
        [SerializeField] EventsControlCanvasInteractionTextSO interactWithRCSO;
        [SerializeField] List<GameEventListener> eventListenersToToggle;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasSetInteractionOptions;
        public GameEventSO hasReachedNews;

        [Header("TV Components")]
        public Renderer tvScreenRenderer;
        public VideoPlayer mainVideoPlayer;

        private Material tvScreenMaterial;
        private GameObject remoteControl;
        private Animator animator = null;

        private int newsChannelIndex = -1;
        private bool playerIsInteracting;
        private bool playerIsAllowedToInteract;
        private float tvVolume;

        private int currentChannelIndex;
        private bool isChangingChannel;
        private bool tvInitialized;

        private void Awake()
        {
            // Crear material dinámico para la pantalla
            tvScreenMaterial = new Material(tvScreenRenderer.material);
            tvScreenRenderer.material = tvScreenMaterial;

            // Configurar video player
            mainVideoPlayer.isLooping = true;
            mainVideoPlayer.source = VideoSource.VideoClip;
        }

        private void OnEnable()
        {
            foreach (GameEventListener listener in eventListenersToToggle)
            {
                listener.enabled = true;
            }

            currentChannelIndex = initialChannelIndex;
            onControlCanvasSetInteractionOptions.Raise(this, interactWithTVSO);
            playerIsInteracting = false;
            playerIsAllowedToInteract = true;
            StartCoroutine(InitializeTV());
        }

        private IEnumerator InitializeTV()
        {
            // Buscar canal de noticias
            for (int i = 0; i < channels.Count; i++)
            {
                if (channels[i].isNewsChannel)
                {
                    newsChannelIndex = i;
                    break;
                }
            }

            // Precargar canal inicial
            yield return StartCoroutine(PrepareChannel(currentChannelIndex));
            PlayCurrentChannel();
            tvVolume = maxTvVolume;
            tvInitialized = true;
        }

        private void Update()
        {
            if (!tvInitialized) return;

            // Interacción básica con la TV
            if (playerIsAllowedToInteract && Input.GetKeyDown(KeyCode.E))
            {
                TogglePlayerInteraction();
            }

            // Controles del TV cuando está interactuando
            if (playerIsAllowedToInteract && playerIsInteracting && !isChangingChannel)
            {
                HandleTVControls();
            }
        }

        private void TogglePlayerInteraction()
        {
            playerIsInteracting = !playerIsInteracting;

            if (playerIsInteracting)
            {
                onControlCanvasSetInteractionOptions.Raise(this, interactWithRCSO);
                ShowRemoteControl(true);
            }
            else
            {
                onControlCanvasSetInteractionOptions.Raise(this, interactWithTVSO);
                ShowRemoteControl(false);
            }
        }

        private void HandleTVControls()
        {
            // Control de volumen
            if (Input.GetKeyDown(KeyCode.UpArrow) && tvVolume < maxTvVolume)
            {
                tvVolume = Mathf.Clamp01(tvVolume + 0.1f);
                mainVideoPlayer.SetDirectAudioVolume(0, tvVolume);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                tvVolume = Mathf.Clamp01(tvVolume - 0.1f);
                mainVideoPlayer.SetDirectAudioVolume(0, tvVolume);
            }
            // Cambio de canal
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(SwitchChannel((currentChannelIndex - 1 + channels.Count) % channels.Count));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(SwitchChannel((currentChannelIndex + 1) % channels.Count));
            }
        }

        private IEnumerator PrepareChannel(int channelIndex)
        {
            TVChannel channel = channels[channelIndex];

            if (!channel.isPrepared)
            {
                mainVideoPlayer.clip = channel.videoClip;
                mainVideoPlayer.targetTexture = channel.renderTexture;
                mainVideoPlayer.Prepare();

                // Esperar a que el video esté preparado
                while (!mainVideoPlayer.isPrepared)
                {
                    yield return null;
                }

                channel.isPrepared = true;
            }
        }

        private void PlayCurrentChannel()
        {
            TVChannel channel = channels[currentChannelIndex];

            // Configurar pantalla
            tvScreenMaterial.mainTexture = channel.renderTexture;

            // Configurar y reproducir video
            mainVideoPlayer.clip = channel.videoClip;
            mainVideoPlayer.targetTexture = channel.renderTexture;
            mainVideoPlayer.time = channel.lastPlaybackTime;
            mainVideoPlayer.SetDirectAudioVolume(0, maxTvVolume);
            mainVideoPlayer.Play();
        }

        private IEnumerator SwitchChannel(int newChannelIndex)
        {
            if (newChannelIndex == currentChannelIndex || isChangingChannel) yield break;

            isChangingChannel = true;

            // Guardar tiempo actual del canal
            channels[currentChannelIndex].lastPlaybackTime = mainVideoPlayer.time;

            // Preparar nuevo canal si es necesario
            if (!channels[newChannelIndex].isPrepared)
            {
                yield return StartCoroutine(PrepareChannel(newChannelIndex));
            }

            // Actualizar canal actual
            currentChannelIndex = newChannelIndex;

            // Cambiar a nuevo canal
            PlayCurrentChannel();
            isChangingChannel = false;

            // Comprobar si es canal de noticias
            if (currentChannelIndex == newsChannelIndex)
            {
                hasReachedNews.Raise(this, null);
                ShowRemoteControl(false);
                playerIsAllowedToInteract = false;
                DisableAllGameEventListeners();
            }
        }

        private void ShowRemoteControl(bool show)
        {
            if (animator == null) return;

            if (show)
            {
                animator.SetTrigger("ShowRemoteControl");
            }
            else
            {
                animator.SetTrigger("HideRemoteControl");
            }
        }

        private void AllowPlayerToInteractWithTV(bool isAllowed)
        {
            playerIsAllowedToInteract = isAllowed;

            if (!isAllowed && playerIsInteracting)
            {
                playerIsInteracting = false;
                ShowRemoteControl(false);
            }
        }

        private void DisableAllGameEventListeners()
        {
            foreach (GameEventListener listener in eventListenersToToggle)
            {
                listener.enabled = false;
            }
        }

        private void SetRemoteControl(Component sender, object data)
        {
            remoteControl = (GameObject)data;
            animator = remoteControl.GetComponentInChildren<Animator>();
        }

        public void PauseGame(Component sender, object data)
        {
            ToogleVideoPlayer((bool)data);
        }

        private void OnDisable()
        {
            // Guardar estado actual al desactivar
            if (tvInitialized && currentChannelIndex < channels.Count)
            {
                channels[currentChannelIndex].lastPlaybackTime = mainVideoPlayer.time;
            }

            if (mainVideoPlayer != null)
            {
                mainVideoPlayer.Stop();
            }

            // Desactivar listeners
            foreach (GameEventListener listener in eventListenersToToggle)
            {
                listener.enabled = false;
            }
        }

        private void ToogleVideoPlayer(bool pause)
        {
            if (pause) { mainVideoPlayer.Pause(); }
            else { mainVideoPlayer.Play(); }
        }

        public void TVAudioFadeOut(Component sender, object data)
        {
            if (data is float fadeDuration)
            {
                StartCoroutine(AudioFadeOutCoroutine(fadeDuration));
            }
            else
            {
                Debug.LogError("[TVHandler] TVAudioFadeOut: data no es un float válido");
            }
        }

        private IEnumerator AudioFadeOutCoroutine(float fadeDuration)
        {
            float startVolume = tvVolume;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / fadeDuration);

                tvVolume = Mathf.Lerp(startVolume, 0f, progress);
                mainVideoPlayer.SetDirectAudioVolume(0, tvVolume);

                yield return null;
            }

            tvVolume = 0f;
            mainVideoPlayer.SetDirectAudioVolume(0, 0f);
        }
    }
}