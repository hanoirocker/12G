using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class WalkieTalkie : PlayerItemBase
    {
        [Header("WalkieTalkie Settings")]
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieEveningData;
        [SerializeField] private WalkieTalkieDataSO[] walkieTalkieNightData;

        private WalkieTalkieDataSO currentWalkieTalkieData;
        private bool canSwitchChannel = true;
        private int currentChannelIndex = 3;
        private int currentDataIndex = 0;

        void Update()
        {
            if (itemIsShown && canSwitchChannel)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SwitchChannel(-1);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SwitchChannel(+1);
                }
            }
        }

        // Lógica para cambiar de canal en el walkie-talkie
        private void SwitchChannel(int direction)
        {
            // Chequea ultima posicion del canal escuchado

            // si el canal no es el ultimo o el primero, cambia al siguiente o anterior canal
            // si el canal posee un audio especial, reproduce el audio especial primero y luego el audio normal del canal
            // que queda en loop. Se debe guardar el instante de tiempo en el que se cambia de canal para continuar el audio especial o normal
            Debug.Log("Canal cambiado en dirección: " + direction);
        }

        public void AllowChannelSwitching(bool allow)
        {
            canSwitchChannel = allow;
        }

        public void SetWalkieTalkie(Component sender, object data)
        {
            var gameContext = (EventContextData)data;
            SceneEnum sceneEnum = gameContext.sceneEnum;
            EventsEnum eventEnum = gameContext.eventEnum;

            if (sceneEnum == SceneEnum.Evening && walkieTalkieEveningData != null && walkieTalkieEveningData.Length > 0)
            {
                for (int i = 0; i < walkieTalkieEveningData.Length; i++)
                {
                    if (walkieTalkieEveningData[i].eventName == eventEnum)
                    {
                        currentDataIndex = i;
                        break;
                    }
                }
                currentWalkieTalkieData = walkieTalkieEveningData[currentDataIndex];
            }
            else if (sceneEnum == SceneEnum.Night && walkieTalkieNightData != null && walkieTalkieNightData.Length > 0)
            {
                for (int i = 0; i < walkieTalkieNightData.Length; i++)
                {
                    if (walkieTalkieNightData[i].eventName == eventEnum)
                    {
                        currentDataIndex = i;
                        break;
                    }
                }
                currentWalkieTalkieData = walkieTalkieNightData[currentDataIndex];
            }

            Debug.Log($"Data set: {currentWalkieTalkieData.name} at index: {currentDataIndex}.");
        }
    }
}