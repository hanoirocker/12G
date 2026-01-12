using System.Collections;
using System.Collections.Generic;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.AudioController
{
    public enum PlayerSoundsType
    {
        HeartBeat,
        NormalBreathing,
        HardBreathing,
        RecoveringBreathing,
        StomachGrowl
    }

    public class PlayerSoundsHandler : MonoBehaviour
    {
        [Header("Footsteps Clips")]
        public List<AudioClip> woodFS;
        public List<AudioClip> carpetFS;
        public List<AudioClip> mosaicGarageFS;
        public List<AudioClip> mosaicBathroomFS;

        [Header("Other Clips")]
        [SerializeField] private AudioClip heartBeatClip;
        [SerializeField, Range(0f, 1f)] private float heartBeatVolume = 0.2f;
        [SerializeField] private AudioClip breathingClip;
        [SerializeField, Range(0f, 1f)] private float breathingVolume = 0.7f;
        [SerializeField] private AudioClip stomachGrowlClip;
        [SerializeField, Range(0f, 1f)] private float stomachGrowlVolume = 0.7f;

        [Header("Footsteps Settings")]
        [SerializeField] private float walkPitch;
        [SerializeField] private float runPitchMin;
        [SerializeField] private float runPitchMax;
        [SerializeField] private float walkCooldown;
        [SerializeField] private float runCooldown;

        private float nextStepTime = 0f;
        private bool isRunning = false;

        private enum FSMaterial
        {
            Wood, Carpet, MosaicGarage, MosaicBathroom, Empty
        }

        private AudioSource audioSource;
        private FPController fpController;

        private void Awake()
        {
            fpController = GetComponent<FPController>();
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.pitch = walkPitch;
        }

        private void Update()
        {
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
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);

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

            audioSource.pitch = isRunning
                ? Random.Range(runPitchMin, runPitchMax)
                : walkPitch;

            audioSource.PlayOneShot(clip);
        }

        public void PlayPlayerSounds(PlayerSoundsType soundType, bool loop, float timeUntilFadeOut, float fadeOutTime)
        {
            switch (soundType)
            {
                case PlayerSoundsType.HeartBeat:
                    StartCoroutine(PlayFadedPlayerSoundCoroutine(heartBeatClip, loop, heartBeatVolume, timeUntilFadeOut, fadeOutTime));
                    break;
                case PlayerSoundsType.NormalBreathing:
                    StartCoroutine(PlayFadedPlayerSoundCoroutine(breathingClip, loop, breathingVolume, timeUntilFadeOut, fadeOutTime));
                    break;
                case PlayerSoundsType.StomachGrowl:
                    StartCoroutine(PlaySimplePlayerSound(stomachGrowlClip, stomachGrowlVolume));
                    break;
                default:
                    break;
            }
        }

        private IEnumerator PlaySimplePlayerSound(AudioClip audioClip, float volume)
        {
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
            AudioSourceState sourceState = source.GetSnapshot();
            source.clip = audioClip;
            source.volume = volume;
            source.Play();
            Debug.Log($"Playing stomach sound");
            yield return new WaitForSeconds(audioClip.length);
            AudioUtils.StopAndRestoreAudioSource(source, sourceState);
            yield return null;
        }

        // TODO: actualmente invocado directamente desde UnwiredEvent.cs, evaluar si dejarlo así
        // o llamar desde VFX si se combina con algún efecto visual.
        private IEnumerator PlayFadedPlayerSoundCoroutine(AudioClip clip, bool loop, float volume, float timeUntilFadeOut, float fadeOutTime)
        {
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
            AudioSourceState sourceState = source.GetSnapshot();
            source.clip = clip;
            source.volume = volume;
            source.loop = loop;
            source.Play();
            yield return new WaitForSeconds(timeUntilFadeOut);
            yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(source, fadeOutTime));
            AudioUtils.StopAndRestoreAudioSource(source, sourceState);
            yield return null;
        }
    }
}
