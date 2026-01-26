using UnityEngine;
using TwelveG.GameController;

namespace TwelveG.InteractableObjects
{
    public class WalkieTalkieDataHandler : MonoBehaviour
    {
        [Header("Data SO References")]
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieEveningData;
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieNightData;
        [SerializeField] private WalkieTalkieDataSO walkieTalkieFreeRoamData;

        /// Convierte la data estática (SO) en una lista de canales Runtime lista para usar.
        public WalkieTalkieChannel[] BuildChannels(WalkieTalkieDataSO data)
        {
            if (data == null) return new WalkieTalkieChannel[0];

            int frequencyCount = data.FrequencyData.Count;
            WalkieTalkieChannel[] channels = new WalkieTalkieChannel[frequencyCount];

            for (int i = 0; i < frequencyCount; i++)
            {
                var soData = data.FrequencyData[i];

                channels[i] = new WalkieTalkieChannel
                {
                    channelIndex = i,
                    staticClip = soData.staticSignalClip, // Mapeamos estática
                    loreClip = soData.loreEventClip,      // Mapeamos lore
                    reactionDialog = soData.reactionDialog, // Mapeamos reacción
                    hasPlayedLore = false, // Reseteamos el flag al cargar nueva data
                    pendingDialog = null
                };
            }

            return channels;
        }

        /// Recibe la data cruda del evento y devuelve el SO correcto.
        public WalkieTalkieDataSO ResolveDataSO(object data)
        {
            // Caso 1: String directo
            if (data is string strData && strData == "FreeRoam")
            {
                return walkieTalkieFreeRoamData;
            }

            // Caso 2: Contexto de Evento
            if (data is EventContextData context)
            {
                if (context.sceneEnum == SceneEnum.Evening)
                {
                    return FindDataByEvent(walkieTalkieEveningData, context.eventEnum);
                }
                else if (context.sceneEnum == SceneEnum.Night)
                {
                    return FindDataByEvent(walkieTalkieNightData, context.eventEnum);
                }
            }

            // Caso 3: Fallback o error
            return null;
        }

        public WalkieTalkieDataSO GetDefaultData()
        {
            // Útil para inicialización de prueba si no hay evento
            if (walkieTalkieEveningData != null && walkieTalkieEveningData.Length > 0)
                return walkieTalkieEveningData[0];
            return null;
        }

        // Buscar en los arrays (evita repetir el bucle for)
        private WalkieTalkieDataSO FindDataByEvent(WalkieTalkieDataSO[] dataArray, EventsEnum targetEvent)
        {
            if (dataArray == null) return null;

            for (int i = 0; i < dataArray.Length; i++)
            {
                if (dataArray[i].eventName == targetEvent)
                {
                    return dataArray[i];
                }
            }
            return null;
        }
    }
}