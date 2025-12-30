using System.Collections;
using TwelveG.AudioController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.GameController
{
    public class LockedUpEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO[] observationTextSOs;

        [Space]
        [Header("Audio Options")]
        [SerializeField] private AudioClip hauntingSoundClip;
        [SerializeField, Range(0f, 1f)] private float hauntingSoundVolume = 0.15f;

        [Space]
        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onPlayerDoorUnlock;

        private bool walkieTalkiePickedUp = false;
        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            // DESACTIVAR SPRINT DEL JUGADOR DURANTE PRIMEROS EVENTOS
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerSprint(false));
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));

            yield return new WaitForSeconds(10f);
            // Supongo que una linterna me vendria bien ..
            if (!walkieTalkiePickedUp)
            {
                GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSOs[0]);
            }

            // Recibe "onWalkieTalkiePickedUp" para permitir la siguiente acción
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
            GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);

            yield return new WaitForSeconds(6f);
            // Ruido de desbloqueo de la puerta
            onPlayerDoorUnlock.Raise(this, null);

            yield return new WaitForSeconds(2f);

            // Comienza musica "Haunting Sound"
            AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);

            if (bgMusicSource != null && hauntingSoundClip != null)
            {
                bgMusicSource.clip = hauntingSoundClip;
                bgMusicSource.volume = 0f;
                bgMusicSource.loop = true;
                bgMusicSource.Play();

                StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(bgMusicSource, 0f, hauntingSoundVolume, 10f));
            }

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

        public void OnWalkieTalkiePickedUp(Component sender, object data)
        {
            walkieTalkiePickedUp = true;
            allowNextAction = true;
        }
    }
}