using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class UnwiredEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 0;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;
        [SerializeField] private ObservationTextSO[] observationTextSOs;

        [Space]
        [Header("Audio Options")]
        [SerializeField] private AudioClip hauntingSoundClip;
        [SerializeField, Range(0f, 1f)] private float hauntingSoundVolume = 0.15f;
        [Space(5)]
        [SerializeField] private AudioClip garageClip;
        [SerializeField, Range(0f, 1f)] private float garageClipVolume = 0.5f;
        [Space(5)]
        [SerializeField] private AudioClip neckWhisperClip;
        [SerializeField, Range(0f, 1f)] private float neckWhisperVolume = 0.35f;

        private bool playerHasNotEnteredGarage = true;
        private bool flashlightPickedUp = false;
        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            PlayerSoundsHandler playerSoundsHandler = FindAnyObjectByType<PlayerSoundsHandler>();
            AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
            Transform garageTransform = GameObject.FindGameObjectWithTag("GarageNoise").transform;
            (AudioSource garageSource, AudioSourceState garageState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(garageTransform, garageClipVolume);
            RotativeDoorHandler garageDoorHandler = GameObject.Find("Garage Rotative Door").GetComponentInChildren<RotativeDoorHandler>();

            yield return new WaitForSeconds(initialTime);

            // Comienza musica "Haunting Sound"
            if (bgMusicSource != null && hauntingSoundClip != null)
            {
                bgMusicSource.clip = hauntingSoundClip;
                bgMusicSource.volume = 0f;
                bgMusicSource.loop = true;
                bgMusicSource.Play();

                StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(bgMusicSource, 0f, hauntingSoundVolume, 10f));
            }

            // Corrutina que hace parpadear las luces de la casa mientras el jugador no haya entrado al garage
            // finalmente corta la luz cuando toque los colliders.
            StartCoroutine(FlickeringLightsAndPowerOutage());

            // "unwiredEntranceCollidersTriggered" (El jugador ya está en la entrada de su casa)
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Objtener el transform del objeto en la escena con tag "GarageNoise"
            // Disparamos el sonido de ruido en el garage
            if (garageTransform)
            {
                if (garageSource != null && garageClip != null)
                {
                    garageSource.clip = garageClip;
                    garageSource.loop = false;
                    garageSource.Play();
                }

                yield return new WaitUntil(() => !garageSource.isPlaying);
                AudioUtils.StopAndRestoreAudioSource(garageSource, garageState);
            }

            // "unwiredGarageCollidersTriggered" (El jugador ya está en la entrada del garage)
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Cerramos la puerta del garage fuertemente
            if (garageDoorHandler != null)
            {
                garageDoorHandler.StrongClosing();
            }

            yield return new WaitForSeconds(0.45f);
            if (playerSoundsHandler)
            {
                playerSoundsHandler.PlayPlayerSounds(PlayerSoundsType.HeartBeat, false, 0.2f, 5f, 3f);
            }

            // Cortar la luz de la casa
            GameEvents.Common.onEnablePlayerHouseEnergy.Raise(this, false);

            yield return new WaitForSeconds(8f);

            // Observación sobre que ya no hay luz en la casa
            GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSOs[0]);
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                observationTextSOs[0].observationTextsStructure[0].observationText
            ));

            yield return new WaitForSeconds(5f);

            if (!flashlightPickedUp)
            {
                // Observación sobre recordar tener una linterna en algún lado del garage
                GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSOs[1]);
                yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                    observationTextSOs[1].observationTextsStructure[0].observationText
                ));
                GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);
            }

            // "onFlashlightPickedUp" (El jugador ha recogido la linterna)
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            AudioSource hauntingSource = AudioManager.Instance.PoolsHandler.ReturnActiveSourceByType(AudioPoolType.BGMusic);
            if (hauntingSource != null)
            {
                 yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(hauntingSource, 5f));
            }

            // esperamos un segundo y disparamos audio en la nuca del jugador
            yield return new WaitForSeconds(7f);
            AudioSource neckSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);

            if (neckSource != null && neckWhisperClip != null)
            {
                AudioSourceState neckState = neckSource.GetSnapshot();
                neckSource.clip = neckWhisperClip;
                neckSource.volume = neckWhisperVolume;
                neckSource.loop = false;
                neckSource.Play();
                yield return new WaitUntil(() => !neckSource.isPlaying);
                AudioUtils.StopAndRestoreAudioSource(neckSource, neckState);
            }

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
        }

        private IEnumerator FlickeringLightsAndPowerOutage()
        {
            while (playerHasNotEnteredGarage)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 25f));
                // Parpadean luces de la casa mientras el jugador no haya colisionado con los colliders
                // del garage
                GameEvents.Common.triggerHouseLightsFlickering.Raise(this, 5f);
            }

            yield return null;
        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

        public void OnPlayerEnteredGarage(Component sender, object data)
        {
            playerHasNotEnteredGarage = false;
            allowNextAction = true;
        }

        public void OnFlashlightPickedUp(Component sender, object data)
        {
            flashlightPickedUp = true;
            GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.Flashlight);
            allowNextAction = true;
        }
    }
}