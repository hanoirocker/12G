using System.Collections.Generic;
using UnityEngine;
using TwelveG.GameController;
using UnityEditor.Timeline.Actions;

namespace TwelveG.VFXController
{
    [System.Serializable]
    public struct SceneVFXSettings
    {
        [Tooltip("El evento asociado a esta configuración")]
        public EventsEnum eventEnum;

        [Tooltip("Intensidad de efecto (0 a 1)")]
        [Range(0f, 1f)]
        public float effectIntensity;

        [Tooltip("Intensidad de fuentes VFX (0 a 1)")]
        [Range(0f, 1f)]
        public float volumeCoefficient;

        public void Deconstruct(out float intensity, out float volume)
        {
            intensity = effectIntensity;
            volume = volumeCoefficient;
        }
    }

    [CreateAssetMenu(fileName = "VFXEventConfigs", menuName = "SO's/Data Structures/VFX Event Configs")]
    public class VFXGeneralConfigSO : ScriptableObject
    {
        [Header("Scene VFX Settings")]
        [SerializeField] private List<SceneVFXSettings> sceneSettings;

        // Ahora el diccionario guarda la ESTRUCTURA COMPLETA, no solo un float
        private Dictionary<EventsEnum, SceneVFXSettings> _settingsDictionary;

        private void InitializeDictionary()
        {
            _settingsDictionary = new Dictionary<EventsEnum, SceneVFXSettings>();

            if (sceneSettings == null)
                sceneSettings = new List<SceneVFXSettings>();

            foreach (var setting in sceneSettings)
            {
                if (!_settingsDictionary.ContainsKey(setting.eventEnum))
                {
                    // Guardamos toda la struct
                    _settingsDictionary.Add(setting.eventEnum, setting);
                }
            }
        }

        // Cambiamos el tipo de retorno de float a SceneVFXSettings
        public SceneVFXSettings GetVFXSettingsForEvenum(EventsEnum eventEnum)
        {
            if (_settingsDictionary == null)
            {
                InitializeDictionary();
            }

            if (_settingsDictionary.TryGetValue(eventEnum, out SceneVFXSettings settings))
            {
                return settings;
            }

            Debug.LogWarning($"[VFXConfig]: No hay config para {eventEnum}. Retornando valores default (0).");

            // Retornamos una struct vacía (todos sus valores serán 0 por defecto)
            return new SceneVFXSettings();
        }
    }
}