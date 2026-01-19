using System.Collections;
using System.Collections.Generic;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.AudioController
{
    public enum PlayerSoundsType
    {
        WakeUpAfternoon,
        FallAsleepAfternoon,
        StandUpEvening,
        ChairMoving,
        StandUpNight,
        UnwiredNeckWhisper,
        VisionsNeckWhisper,
        HeartBeat,
        NormalBreathing,
        HardBreathing, // Ya incluye al final la recuperacion
        StomachGrowl,
        EnemySurpriseReaction, // TODO: Componer sonido de sorpresa enemigo (Sorpresa + Respiración agitada + Latidos + Recuperacion del aire)
        Doubt, // Hmmm?
    }

    public enum FSMaterial
    {
        Empty,
        Wood,
        Carpet,
        MosaicGarage,
        MosaicBathroom
    }

    public class PlayerSoundsHandler : MonoBehaviour
    {
        [Header("Footsteps Clips")]
        public List<AudioClip> woodFS;
        public List<AudioClip> carpetFS;
        public List<AudioClip> mosaicGarageFS;
        public List<AudioClip> mosaicBathroomFS;

        [Space(10)]
        [Header("Other Clips")]
        [SerializeField] private AudioClip wakeUpAfternoonClip;
        [SerializeField, Range(0f, 1f)] private float wakeUpAfternoonClipVolume = 0.7f;
        [SerializeField] private AudioClip fallAsleepAfternoonClip;
        [SerializeField, Range(0f, 1f)] private float fallAsleepAfternoonClipVolume = 0.7f;
        [SerializeField] private AudioClip standUpEveningClip;
        [SerializeField, Range(0f, 1f)] private float standUpEveningClipVolume = 0.7f;
        [SerializeField] private AudioClip chairMovingClip;
        [SerializeField, Range(0f, 1f)] private float chairMovingClipVolume = 0.8f;
        [SerializeField] private AudioClip standUpNightClip;
        [SerializeField, Range(0f, 1f)] private float standUpNightClipVolume = 0.7f;
        [SerializeField] private AudioClip unwiredNeckWhisperClip;
        [SerializeField, Range(0f, 1f)] private float unwiredNeckWhisperClipVolume = 0.35f;
        [SerializeField] private AudioClip visionsNeckWhisperClip;
        [SerializeField, Range(0f, 1f)] private float visionsNeckWhisperClipVolume = 0.4f;
        [SerializeField] private AudioClip heartBeatClip;
        [SerializeField, Range(0f, 1f)] private float heartBeatClipVolume = 0.2f;
        [SerializeField] private AudioClip softBreathingClip;
        [SerializeField, Range(0f, 1f)] private float softBreathingClipVolume = 0.7f;
        [SerializeField] private AudioClip hardBreathingClip;
        [SerializeField, Range(0f, 1f)] private float hardBreathingClipVolume = 0.7f;
        [SerializeField] private AudioClip stomachGrowlClip;
        [SerializeField, Range(0f, 1f)] private float stomachGrowlClipVolume = 0.7f;
        [SerializeField] private AudioClip enemySurprisedClip;
        [SerializeField, Range(0f, 1f)] private float enemySurprisedClipVolume = 0.7f;
        [SerializeField] private AudioClip doubtClip;
        [SerializeField, Range(0f, 1f)] private float doubtClipVolume = 0.7f;

        [Space(10)]
        [Header("Footsteps Settings")]
        [SerializeField] private float walkPitch;
        [SerializeField] private float runPitchMin;
        [SerializeField] private float runPitchMax;
        [SerializeField] private float walkCooldown;
        [SerializeField] private float runCooldown;

        private float nextStepTime = 0f;
        private bool isRunning = false;

        private PlayerHandler playerHandler;
        private AudioSource playerCapsuleAudioSource;
        private FPController fpController;

        private void Update()
        {
            // Nos aseguramos que se haya registrado correctamente el PlayerHandler
            if (fpController == null) return;

            bool isMoving = Input.GetKey(KeyCode.W) ||
                            Input.GetKey(KeyCode.A) ||
                            Input.GetKey(KeyCode.S) ||
                            Input.GetKey(KeyCode.D);

            if (!isMoving) return;

            isRunning = fpController.IsSprinting();

            // Controla cooldown entre pasos
            if (Time.time >= nextStepTime)
            {
                PlayFootStepsSounds(isRunning);
                nextStepTime = Time.time + (isRunning ? runCooldown : walkCooldown);
            }
        }

        private FSMaterial SurfaceSelect()
        {
            RaycastHit hit;
            Ray ray = new Ray(fpController.gameObject.transform.position + Vector3.up * 0.5f, -Vector3.up);

            if (Physics.Raycast(ray, out hit, 1.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Renderer surfaceRenderer = hit.collider.GetComponentInChildren<Renderer>();

                if (surfaceRenderer && surfaceRenderer.sharedMaterial)
                {
                    var matName = surfaceRenderer.sharedMaterial.name;

                    if (matName.Contains("Wood")) return FSMaterial.Wood;
                    if (matName.Contains("Carpet")) return FSMaterial.Carpet;
                    if (matName.Contains("Mosaic - Garage")) return FSMaterial.MosaicGarage;
                    if (matName.Contains("Mosaic - Bathroom")) return FSMaterial.MosaicBathroom;
                }
            }

            return FSMaterial.Empty;
        }

        private void PlayFootStepsSounds(bool isRunning)
        {
            AudioClip clip = null;
            FSMaterial surface = SurfaceSelect();

            switch (surface)
            {
                case FSMaterial.Wood:
                    clip = woodFS[Random.Range(0, woodFS.Count)];
                    break;
                case FSMaterial.MosaicBathroom:
                    clip = mosaicBathroomFS[Random.Range(0, mosaicBathroomFS.Count)];
                    break;
                case FSMaterial.MosaicGarage:
                    clip = mosaicGarageFS[Random.Range(0, mosaicGarageFS.Count)];
                    break;
                case FSMaterial.Carpet:
                    clip = carpetFS[Random.Range(0, carpetFS.Count)];
                    break;
            }
            if (clip == null) return;

            playerCapsuleAudioSource.pitch = isRunning
                ? Random.Range(runPitchMin, runPitchMax)
                : walkPitch;

            playerCapsuleAudioSource.PlayOneShot(clip);
        }

        public void RegisterPlayerHandler(PlayerHandler handler)
        {
            playerHandler = handler;

            fpController = playerHandler?.FPController;

            if (fpController == null)
            {
                Debug.LogWarning("[PlayerSoundsHandler]: Desactivandose por no tener referencia de FPController");
                this.enabled = false;
                return;
            }

            playerCapsuleAudioSource = playerHandler?.PlayerCapsuleAudioSource;

            if (playerCapsuleAudioSource == null)
            {
                Debug.LogWarning("[PlayerSoundsHandler]: No se encontró referencia a AudioSource del PlayerCapsule");
            }
        }

        public IEnumerator PlayPlayerSound(PlayerSoundsType playerSoundsType, float timeUntilFadeOut = 0f, float fadeOutTime = 0f)
        {
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
            AudioSourceState sourceState = source.GetSnapshot();
            (AudioClip audioClipToPlay, float volume) = DetermineAudioClip(playerSoundsType);
            source.clip = audioClipToPlay;
            source.loop = timeUntilFadeOut > 0f;
            source.volume = volume;
            source.Play();

            yield return new WaitForSeconds(timeUntilFadeOut > 0f ? timeUntilFadeOut : audioClipToPlay.length);

            if (timeUntilFadeOut > 0f && fadeOutTime > 0f)
            {
                yield return AudioManager.Instance.FaderHandler.AudioSourceFadeOut(source, fadeOutTime);
                source.loop = false;
            }

            AudioUtils.StopAndRestoreAudioSource(source, sourceState);

            yield return null;
        }

        private (AudioClip, float) DetermineAudioClip(PlayerSoundsType playerSoundsType)
        {
            switch (playerSoundsType)
            {
                case PlayerSoundsType.WakeUpAfternoon:
                    return (wakeUpAfternoonClip, wakeUpAfternoonClipVolume);
                case PlayerSoundsType.FallAsleepAfternoon:
                    return (fallAsleepAfternoonClip, fallAsleepAfternoonClipVolume);
                case PlayerSoundsType.StandUpEvening:
                    return (standUpEveningClip, standUpEveningClipVolume);
                case PlayerSoundsType.ChairMoving:
                    return (chairMovingClip, chairMovingClipVolume);
                case PlayerSoundsType.StandUpNight:
                    return (standUpNightClip, standUpNightClipVolume);
                case PlayerSoundsType.UnwiredNeckWhisper:
                    return (unwiredNeckWhisperClip, unwiredNeckWhisperClipVolume);
                case PlayerSoundsType.VisionsNeckWhisper:
                    return (visionsNeckWhisperClip, visionsNeckWhisperClipVolume);
                case PlayerSoundsType.HeartBeat:
                    return (heartBeatClip, heartBeatClipVolume);
                case PlayerSoundsType.NormalBreathing:
                    return (softBreathingClip, softBreathingClipVolume);
                case PlayerSoundsType.HardBreathing:
                    return (hardBreathingClip, hardBreathingClipVolume);
                case PlayerSoundsType.StomachGrowl:
                    return (stomachGrowlClip, stomachGrowlClipVolume);
                case PlayerSoundsType.EnemySurpriseReaction:
                    return (enemySurprisedClip, enemySurprisedClipVolume);
                case PlayerSoundsType.Doubt:
                    return (doubtClip, doubtClipVolume);
                default:
                    return (null, 0f);
            }
        }
    }
}
