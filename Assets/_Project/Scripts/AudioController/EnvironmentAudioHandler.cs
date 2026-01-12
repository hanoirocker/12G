using System;
using System.Collections;
using System.Collections.Generic;
using TwelveG.EnvironmentController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.AudioController
{
    public class EnvironmentAudioHandler : MonoBehaviour
    {
        [Header("Scene-based Ambient Prefabs")]
        [SerializeField] private List<Transform> afternoonSoundsTransforms;
        [SerializeField] private List<Transform> eveningSoundsTransforms;
        [SerializeField] private List<Transform> nightSoundsTransforms;

        [Space(10)]
        [Header("Area-based Ambient Profiles")]
        [Space(5)]
        [Header("Afternoon profiles")]
        [SerializeField] private List<AmbienceProfileSO> afternoonAreaAmbientProfiles;
        [Space(5)]
        [Header("Evening profiles")]
        [SerializeField] private List<AmbienceProfileSO> eveningAreaAmbientProfiles;
        [Space(5)]
        [Header("Night profiles")]
        [SerializeField] private List<AmbienceProfileSO> nightAreaAmbientProfiles;

        [Space(5)]
        [Header("Global Settings")]
        [SerializeField, Range(0.1f, 5f)] private float fadeDuration = 1f;

        [Space(10)]
        [Header("Debug")]
        public HouseArea currentHouseArea = HouseArea.None;

        // Clase interna para rastrear el estado
        private class ActiveAudioInfo
        {
            public AudioSource source;
            public AudioSourceState originalState;
            public Coroutine fadeCoroutine;
            public float currentTargetVolume;
        }

        private Dictionary<AudioClip, ActiveAudioInfo> activeAmbientSounds = new Dictionary<AudioClip, ActiveAudioInfo>();
        private List<AmbienceProfileSO> currentAreaProfiles = new List<AmbienceProfileSO>();

        private void Start()
        {
            SceneEnum sceneEnum = SceneUtils.RetrieveCurrentSceneEnum();

            switch (sceneEnum)
            {
                case SceneEnum.Afternoon:
                    InitiateEnvironmentSounds(2);
                    break;
                case SceneEnum.Evening:
                    InitiateEnvironmentSounds(3);
                    break;
                case SceneEnum.Night:
                    InitiateEnvironmentSounds(4);
                    break;
                default:
                    DisableAllSceneAmbients();
                    break;
            }
        }

        public void InitiateEnvironmentSounds(float sceneIndex)
        {
            DisableAllSceneAmbients();

            if (sceneIndex == 2)
            {
                ActivateList(afternoonSoundsTransforms);
                currentAreaProfiles = afternoonAreaAmbientProfiles;
            }
            else if (sceneIndex == 3)
            {
                ActivateList(eveningSoundsTransforms);
                currentAreaProfiles = eveningAreaAmbientProfiles;
            }
            else if (sceneIndex == 4)
            {
                ActivateList(nightSoundsTransforms);
                currentAreaProfiles = nightAreaAmbientProfiles;
            }
        }

        private void DisableAllSceneAmbients()
        {
            DeactivateList(afternoonSoundsTransforms);
            DeactivateList(eveningSoundsTransforms);
            DeactivateList(nightSoundsTransforms);
        }

        private void ActivateList(List<Transform> list) { if (list != null) list.ForEach(t => t.gameObject.SetActive(true)); }
        private void DeactivateList(List<Transform> list) { if (list != null) list.ForEach(t => t.gameObject.SetActive(false)); }

        public void OnPlayerTriggeredHouseArea(Component sender, object data)
        {
            HouseArea newArea = (HouseArea)data;
            if (newArea == currentHouseArea) return;

            currentHouseArea = newArea;
            EvaluateAreaSounds(newArea);
        }

        private void EvaluateAreaSounds(HouseArea houseArea)
        {
            // 1. Obtener QUÉ clips deben sonar y a QUÉ VOLUMEN
            // Mapeo de Clip -> Volumen Deseado
            Dictionary<AudioClip, float> targetClipsData = new Dictionary<AudioClip, float>();

            foreach (var profile in currentAreaProfiles)
            {
                // Si el perfil corresponde al área actual
                if (Array.Exists(profile.activeAreas, area => area == houseArea))
                {
                    foreach (var config in profile.ambientClips)
                    {
                        if (config.clip == null) continue;

                        // Si el clip ya fue agregado por otro perfil en la misma área (raro pero posible),
                        // nos quedamos con el volumen más alto (o el último, decisión de diseño).
                        if (!targetClipsData.ContainsKey(config.clip))
                        {
                            targetClipsData.Add(config.clip, config.volume);
                        }
                    }
                }
            }

            // 2. Detectar sonidos a DETENER (Están sonando, pero NO están en targetClipsData)
            List<AudioClip> clipsToRemove = new List<AudioClip>();

            foreach (var clip in activeAmbientSounds.Keys)
            {
                if (!targetClipsData.ContainsKey(clip))
                {
                    clipsToRemove.Add(clip);
                }
            }

            foreach (var clip in clipsToRemove)
            {
                // Debug.Log($"Deteniendo sonido de ambiente: {clip.name} en área {houseArea}");
                FadeOutAndReturn(clip);
            }

            // 3. Detectar sonidos a INICIAR o ACTUALIZAR
            foreach (var kvp in targetClipsData)
            {
                AudioClip clip = kvp.Key;
                float specificVolume = kvp.Value;

                if (!activeAmbientSounds.ContainsKey(clip))
                {
                    // Nuevo sonido -> Fade In
                    // Debug.Log($"Iniciando sonido de ambiente: {clip.name} en área {houseArea}");
                    FadeInAndPlay(clip, specificVolume);
                }
                else
                {
                    // Debug.Log($"Actualizando volumen de de clip {clip.name}");
                    // Si el sonido ya está sonando:
                    CheckUpdateVolume(clip, specificVolume);
                }
            }
        }

        // ----------------------------------------------------------------------
        // CONTROL DE AUDIOSOURCES Y FADES
        // ----------------------------------------------------------------------

        private void CheckUpdateVolume(AudioClip clip, float newTargetVol)
        {
            if (activeAmbientSounds.TryGetValue(clip, out ActiveAudioInfo info))
            {
                // Verificamos si el volumen objetivo ha cambiado significativamente
                // (Usamos 0.01f para evitar actualizaciones por errores flotantes infinitesimales)
                if (Mathf.Abs(info.currentTargetVolume - newTargetVol) > 0.01f)
                {
                    // 1. Actualizamos el registro de cuál es el nuevo volumen deseado
                    info.currentTargetVolume = newTargetVol;

                    // 2. Si ya se estaba haciendo un fade, lo cortamos para que no peleen
                    if (info.fadeCoroutine != null) StopCoroutine(info.fadeCoroutine);

                    // 3. Obtenemos el volumen real actual del AudioSource como punto de partida
                    float currentVol = info.source.volume;

                    // 4. Iniciamos la transición
                    info.fadeCoroutine = StartCoroutine(
                        AudioManager.Instance.FaderHandler.AudioSourceFadeIn(
                            info.source,
                            currentVol,
                            newTargetVol,
                            fadeDuration
                        )
                    );

                    // Debug.Log($"[EnvironmentAudioHandler] Actualizando volumen {clip.name}: {currentVol} -> {newTargetVol}");
                }
            }
        }

        private void FadeInAndPlay(AudioClip clip, float targetVol)
        {
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.HouseStereoAmbient);

            if (source == null) return;

            AudioSourceState state = source.GetSnapshot();

            source.clip = clip;
            source.loop = true;
            source.volume = 0f;
            source.Play();

            ActiveAudioInfo info = new ActiveAudioInfo
            {
                source = source,
                originalState = state,
                currentTargetVolume = targetVol,
                fadeCoroutine = null
            };

            activeAmbientSounds.Add(clip, info);

            info.fadeCoroutine = StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(source, 0f, targetVol, fadeDuration));
        }

        private void FadeOutAndReturn(AudioClip clip)
        {
            if (activeAmbientSounds.TryGetValue(clip, out ActiveAudioInfo info))
            {
                if (info.fadeCoroutine != null) StopCoroutine(info.fadeCoroutine);
                info.fadeCoroutine = StartCoroutine(FadeOutRoutine(clip, info));
            }
        }

        private IEnumerator FadeOutRoutine(AudioClip clip, ActiveAudioInfo info)
        {
            yield return AudioManager.Instance.FaderHandler.AudioSourceFadeOut(info.source, fadeDuration);

            AudioUtils.StopAndRestoreAudioSource(info.source, info.originalState);
            activeAmbientSounds.Remove(clip);
        }
    }
}