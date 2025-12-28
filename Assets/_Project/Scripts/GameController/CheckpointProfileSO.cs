using UnityEngine;
using System.Collections.Generic;
using TwelveG.InteractableObjects;
using TwelveG.AudioController;

namespace TwelveG.GameController
{
    [CreateAssetMenu(fileName = "NewCheckpointProfile", menuName = "TwelveG/Data/Checkpoint Profile")]
    public class CheckpointProfileSO : ScriptableObject
    {
        [Header("Identity")]
        public EventsEnum eventEnum;
        public string chapterName; // Titulo. Pensado para mostrar en el menú de capítulos
        public Sprite chapterThumbnail; // La foto para el menú de capítulos

        [Header("Player State")]
        public List<ItemType> startingInventory;
        public bool flashlightEnabled;

        [Header("World State")]
        public WeatherEvent initialWeather;
        
        [Header("Critical Objects State")]
        // Una lista simple de IDs o nombres de objetos que deben estar
        // en un estado diferente al default de la escena.

        // TODO: trabjar un "StateEnforcer" o algo así para manejar esto mejor.
        public List<string> objectsToDisable; 
        public List<string> objectsToEnable;
        // public bool officeDoorIsOpen; --- IGNORE --- 
    }
}