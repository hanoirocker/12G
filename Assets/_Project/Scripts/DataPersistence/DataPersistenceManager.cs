using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace TwelveG.SaveSystem
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;

        private GameData gameData;

        private List<IDataPersistence> dataPersistenceObjects;
        private FileDataHandler dataHandler;

        public static DataPersistenceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Application.persistentDataPath retorna el directorio de data persitente según
            // cada OS para proyectos en Unity!
            // print("Application.persistentDataPath: " + Application.persistentDataPath);
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadPersistenceData();
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> dataPersistenceObjects =
                FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

            return new List<IDataPersistence>(dataPersistenceObjects);
        }

        // Para testear - La idea no es que el juego se guarde automáticamente
        // Cuando el jugador abandone la partida. O si? hmmmm
        private void OnApplicationQuit()
        {
            SavePersistenceData();
        }

        public void NewGame()
        {
            gameData = new GameData();
        }

        public void SavePersistenceData()
        {
            // Pasar información a otros scripts para actualiar la gameData
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.SaveData(ref gameData);
            }

            // Incrementar valor de savesNumber
            gameData.savesNumber += 1;
            // Guardar data usando el data handler
            dataHandler.Save(gameData);
        }

        public void LoadPersistenceData()
        {
            // Cargar data guardada usando el data handler
            gameData = dataHandler.Load();

            // Si no existe data guardada, inciciar nueva partida
            if (gameData == null)
            {
                Debug.Log("[DataPersistenceManager]: No saved data was found, initializing new game");
                NewGame();
            }

            // Enviar información cargada a todos los
            // managers que necesitan leer dichos valores
            // para iniciar configs (AudioManaer, LocalizationManager, PlayerManager, GameManager ..)
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
    }
}