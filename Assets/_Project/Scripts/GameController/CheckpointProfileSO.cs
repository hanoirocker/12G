using UnityEngine;
using System.Collections.Generic;
using TwelveG.InteractableObjects;
using TwelveG.AudioController;

namespace TwelveG.GameController
{
    [System.Serializable]
    public struct ObjectData
    {
        public string objectID;
        public bool isActive;

        public ObjectData(string id, bool active)
        {
            this.objectID = id;
            this.isActive = active;
        }
    }

    [System.Serializable]
    public struct objectCheckpointData
    {
        public string objectID;
        public string state;

        public objectCheckpointData(string id, string state)
        {
            this.objectID = id;
            this.state = state;
        }
    }

    [CreateAssetMenu(fileName = "Checkpoint - ", menuName = "SO's/Data Structures/Checkpoint Profile")]
    public class CheckpointProfileSO : ScriptableObject
    {
        [Header("Identity")]
        public EventsEnum eventEnum;
        public string chapterName; // Titulo. Pensado para mostrar en el menú de capítulos
        public Sprite chapterThumbnail; // La foto para el menú de capítulos

        [Header("Player State")]
        public List<ItemType> startingInventory;
        [Space(5)]
        public bool flashlightEnabled;
        public bool walkieTalkieEnabled;

        [Header("World State")]
        public WeatherEvent initialWeather;

        [Header("Critical Objects State")]
        [Space(5)]
        // Una lista simple de IDs o nombres de objetos que deben estar
        // en un estado diferente al default de la escena.

        public List<ObjectData> objectsToToggle;
        [Space(5)]
        public List<objectCheckpointData> objectsWithCheckpointListener;
    }
}