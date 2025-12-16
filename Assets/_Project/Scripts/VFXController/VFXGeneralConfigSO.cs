using System.Collections.Generic;
using UnityEngine;
using TwelveG.GameController;

namespace TwelveG.VFXController
{
    [CreateAssetMenu(fileName = "VFXGeneralConfig", menuName = "TwelveG/VFX/General Config")]
    public class VFXGeneralConfigSO : ScriptableObject
    {
        [System.Serializable]
        public struct SceneVFXSettings
        {
            [Tooltip("El evento asociado a esta configuración")]
            public EventsEnum eventEnum;

            [Tooltip("Intensidad de efecto (0 a 1)")]
            [Range(0f, 1f)]
            public float initialHeadacheIntensity;
        }

        [Header("Scene VFX Settings")]
        [SerializeField] private List<SceneVFXSettings> sceneSettings;

        // NO lo serializamos, se reconstruye al jugar
        private Dictionary<EventsEnum, float> _settingsDictionary;

        private void InitializeDictionary()
        {
            _settingsDictionary = new Dictionary<EventsEnum, float>();

            // Protección extra: Si la lista está vacía o es nula en el inspector
            if (sceneSettings == null)
                sceneSettings = new List<SceneVFXSettings>();

            foreach (var setting in sceneSettings)
            {
                // Evitamos duplicados para que no crashee si pones dos veces el mismo evento en la lista
                if (!_settingsDictionary.ContainsKey(setting.eventEnum))
                {
                    _settingsDictionary.Add(setting.eventEnum, setting.initialHeadacheIntensity);
                }
            }
        }

        public float GetIntensityForScene(EventsEnum eventEnum)
        {
            if (_settingsDictionary == null)
            {
                InitializeDictionary();
            }

            if (_settingsDictionary.TryGetValue(eventEnum, out float intensity))
            {
                return intensity;
            }

            // Si no hay configuración para este evento, devolvemos 0 (sin dolor)
            Debug.LogWarning($"[VFXConfig]: No hay config para {eventEnum}. Intensidad 0.");
            return 0f;
        }
    }
}