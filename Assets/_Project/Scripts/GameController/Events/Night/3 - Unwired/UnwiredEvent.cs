using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
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

        private bool playerHasNotEnteredGarage = true;
        private bool flashlightPickedUp = false;
        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            PlayerHouseHandler playerHouseHandler = PlayerHouseHandler.Instance;
            PlayerSoundsHandler playerSoundsHandler = AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>();
            AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
            Transform garageTransform = playerHouseHandler.GetTransformByObject(HouseObjects.GarageNoise);
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
                (AudioSource garageSource, AudioSourceState garageState) =
                    AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                        garageTransform, garageClipVolume
                );

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

            // Espera lo que tardaría la puerta en cerrarse fuertemente y corta la luz de la casa
            yield return new WaitForSeconds(0.45f);
            GameEvents.Common.onEnablePlayerHouseEnergy.Raise(this, false);

            if (playerSoundsHandler)
            {
                StartCoroutine(playerSoundsHandler.PlayPlayerSound(PlayerSoundsType.HeartBeat, 5f, 3f));
            }

            yield return new WaitForSeconds(8f);

            // Observación sobre que ya no hay luz en la casa
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                observationTextSOs[0]
            );
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                observationTextSOs[0].observationTextsStructure[0].observationText
            ));

            yield return new WaitForSeconds(5f);

            if (!flashlightPickedUp)
            {
                // Observación sobre recordar tener una linterna en algún lado del garage
                UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                    observationTextSOs[1]
                );
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

            // esperamos unos segundos y disparamos audio en la nuca del jugador
            yield return new WaitForSeconds(7f);
            yield return StartCoroutine(
                playerSoundsHandler.PlayPlayerSound(PlayerSoundsType.UnwiredNeckWhisper)
            );
            yield return new WaitForSeconds(4f);
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