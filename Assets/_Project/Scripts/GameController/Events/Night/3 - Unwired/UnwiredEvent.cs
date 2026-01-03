using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.GameController
{
    public class UnwiredEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 0;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO[] observationTextSOs;
        [SerializeField] private DialogSO[] dialogSOs;

        [Space]
        [Header("Audio Options")]
        [SerializeField] private AudioClip hauntingSoundClip;
        [SerializeField, Range(0f, 1f)] private float hauntingSoundVolume = 0.15f;
        [Space(5)]
        [SerializeField] private AudioClip garageClip;
        [SerializeField, Range(0f, 1f)] private float garageClipVolume = 0.5f;

        [Space]
        [Header("Game Event SO's")]
        // [SerializeField] private GameEventSO onPlayerDoorUnlock;

        private bool flashlightPickedUp = false;
        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
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

            // "unwiredCollidersTriggered" (El jugador ya está en la entrada de su casa)
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

            // TODO: Desactivar el RotativeDoorHandler y activar el DownstairsOfficeDoorHandler en la misma puerta?
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Titilan las luces varias veces y luego el apagón

            // Simon intenta llamar a Mica

            // Observación sobre la lintera en el garage

            // "onFlashlightPickedUp" (El jugador ha recogido la linterna)
            // esperamos un segundo y disparamos audio en la nuca del jugador

        }

        public void AllowNextActions(Component sender, object data)
        {
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

        public void OnFlashlightPickedUp(Component sender, object data)
        {
            flashlightPickedUp = true;
            allowNextAction = true;
        }
    }
}