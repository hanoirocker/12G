namespace TwelveG.PlayerController
{
    using System.Collections.Generic;
    using TwelveG.Localization;
    using TwelveG.UIController;
    using UnityEngine;
    using UnityEngine.Video;

    public class TVHandler : MonoBehaviour
    {
        public int initialChannelIndex = 0;

        [SerializeField] List<GameObject> planes;
        [SerializeField] List<VideoPlayer> videoPlayers;
        [SerializeField] EventsControlCanvasInteractionTextSO interactWithTVSO;
        [SerializeField] EventsControlCanvasInteractionTextSO interactWithRCSO;
        [SerializeField] List<GameEventListener> eventListenersToToggle;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasSetInteractionOptions;
        public GameEventSO hasReachedNews;

        private GameObject remoteControl;
        private Animator animator = null;

        private int newsChannelIndex;
        private bool tvIsActive;
        private bool playerIsInteracting;
        private bool playerIsAllowedToInteract;
        private float tvVolume = 1f;

        private int currentChannelIndex;
        private int previousChannelIndex;

        private void OnEnable()
        {
            foreach (GameEventListener gameEventListener in eventListenersToToggle)
            {
                gameEventListener.enabled = true;
            }

            currentChannelIndex = initialChannelIndex;
            onControlCanvasSetInteractionOptions.Raise(this, interactWithTVSO);
            ToogleTV();
        }

        private void Start()
        {
            previousChannelIndex = currentChannelIndex;
            playerIsInteracting = false;
            playerIsAllowedToInteract = false;
            newsChannelIndex = GetMainVideoIndex();
        }

        private void Update()
        {
            if (playerIsAllowedToInteract && Input.GetKeyDown(KeyCode.E))
            {
                PlayerState(!playerIsInteracting);
                PlayRemoteControlClips();
            }

            if (playerIsInteracting)
            {
                TVInteraction();
            }

            HasReachedNews();
            return;
        }

        private int GetMainVideoIndex()
        {
            for (int i = 0; i < videoPlayers.Count; i++)
            {
                if (videoPlayers[i].gameObject.CompareTag("Main Video"))
                {
                    return i;
                }
            }
            return 0;
        }


        private void PlayRemoteControlClips()
        {
            if (playerIsInteracting)
            {
                animator.SetTrigger("ShowRemoteControl");
            }
            else
            {
                animator.SetTrigger("HideRemoteControl");
            }
        }

        private void TVInteraction()
        {
            // Volumen up
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (tvVolume > 1) { tvVolume = 1; }

                if (tvVolume >= 0 && tvVolume < 1f)
                {
                    tvVolume += 0.1f;
                }
                else
                {
                    return;
                }
                videoPlayers[currentChannelIndex].SetDirectAudioVolume(0, tvVolume);
            }
            // Volumen down
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (tvVolume < 0) { tvVolume = 0; }

                if (tvVolume > 0 && tvVolume <= 1f)
                {
                    tvVolume -= 0.1f;
                }
                else
                {
                    return;
                }
                videoPlayers[currentChannelIndex].SetDirectAudioVolume(0, tvVolume);
            }
            // Previous channel
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                previousChannelIndex = currentChannelIndex;

                if (currentChannelIndex - 1 < 0)
                {
                    currentChannelIndex = planes.Count - 1;
                }
                else
                {
                    currentChannelIndex -= 1;
                }
                // animator.SetTrigger("ChangeChannel");
                planes[previousChannelIndex].SetActive(false);
                videoPlayers[currentChannelIndex].SetDirectAudioVolume(0, tvVolume);
                planes[currentChannelIndex].SetActive(true);
                UnmuteCurrentVideoPlayer(currentChannelIndex);
            }
            // Next channel
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                previousChannelIndex = currentChannelIndex;

                if (currentChannelIndex + 1 > planes.Count - 1)
                {
                    currentChannelIndex = 0;
                }
                else
                {
                    currentChannelIndex += 1;
                }
                // TODO: Hacer que la animacion Change Channel vuelva al estado previo
                planes[previousChannelIndex].SetActive(false);
                videoPlayers[currentChannelIndex].SetDirectAudioVolume(0, tvVolume);
                planes[currentChannelIndex].SetActive(true);
                UnmuteCurrentVideoPlayer(currentChannelIndex);
            }
        }

        private void PlayerState(bool playerStateOfInteraction)
        {
            playerIsInteracting = playerStateOfInteraction;

            if (playerIsInteracting)
            {
                onControlCanvasSetInteractionOptions.Raise(this, interactWithRCSO);
            }
            else
            {
                onControlCanvasSetInteractionOptions.Raise(this, interactWithTVSO);
            }
        }

        private void AllowPlayerToInteractWithTV(bool isPlayerAllowedToInteract)
        {
            if (isPlayerAllowedToInteract == false)
            {
                playerIsAllowedToInteract = false;
                playerIsInteracting = false;
                PlayRemoteControlClips();
            }
            else
            {
                playerIsAllowedToInteract = isPlayerAllowedToInteract;
            }
        }

        private void HasReachedNews()
        {
            if (currentChannelIndex == newsChannelIndex)
            {
                hasReachedNews.Raise(this, null);
                DisableAllGameEventListeners();
                this.enabled = false;
            }
        }

        private void DisableAllGameEventListeners()
        {
            GameEventListener[] listeners = GetComponents<GameEventListener>();
            foreach (GameEventListener listener in listeners)
            {
                listener.enabled = false;
            }
        }

        private void ToogleTV()
        {
            planes[currentChannelIndex].SetActive(true);
            UnmuteCurrentVideoPlayer(currentChannelIndex);
        }

        private void SetRemoteControl(Component sender, object data)
        {
            print("Recibido en TVHandler, intentando SetRemoteControl, data: " + data);
            remoteControl = (GameObject)data;
            animator = remoteControl.GetComponentInChildren<Animator>();
        }

        private void UnmuteCurrentVideoPlayer(int channelIndex)
        {
            for (int i = 0; i < videoPlayers.Count; i++)
            {
                if (i != channelIndex)
                {
                    videoPlayers[i].SetDirectAudioMute(0, true);
                }
                else
                {
                    videoPlayers[i].SetDirectAudioMute(0, false);
                }
            }
        }

        public void OnPauseMenuToggle()
        {
            if (this.isActiveAndEnabled)
            {
                if (PauseMenuCanvasHandler.gameIsPaused)
                {
                    PauseVideoPlayers();
                }
                else
                {
                    ResumeVideoPlayers();
                }
            }
        }

        private void ResumeVideoPlayers()
        {
            foreach (VideoPlayer videoPlayer in videoPlayers)
            {
                videoPlayer.Play();
            }
        }

        private void PauseVideoPlayers()
        {
            foreach (VideoPlayer videoPlayer in videoPlayers)
            {
                videoPlayer.Pause();
            }
        }

        private void OnDisable()
        {
            foreach (GameEventListener gameEventListener in eventListenersToToggle)
            {
                gameEventListener.enabled = false;
            }
        }
    }
}