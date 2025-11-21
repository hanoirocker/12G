namespace TwelveG.SaveSystem
{
  using UnityEngine;
  using System;
  using System.IO;

  public class FileDataHandler
  {
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
      this.dataDirPath = dataDirPath;
      this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
      string fullPath = Path.Combine(dataDirPath, dataFileName);

      GameData loadedData = null;

      if (File.Exists(fullPath))
      {
        try
        {
          // Cargar data serializada desde el archivo
          string dataToLoad = "";
          using (FileStream stream = new FileStream(fullPath, FileMode.Open))
          {
            using (StreamReader reader = new StreamReader(stream))
            {
              dataToLoad = reader.ReadToEnd();
            }
          }

          // Deserealizar
          loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

        }
        catch (Exception e)
        {
          Debug.LogError("Error ocurred when trying to load data from file: " + fullPath + "\n" + e);
        }
      }

      return loadedData;
    }

    public void Save(GameData data)
    {
      // Prevenir diferencias en los path separators entre sistemas operativos .. usamos Combine
      string fullPath = Path.Combine(dataDirPath, dataFileName);

      try
      {
        // Crear directorio de guardado en caso de que no exista
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        // Serializar game data en JSON (true para darle formato)
        string dataToStore = JsonUtility.ToJson(data, true);

        // Escribimos el archivo al SO
        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
        {
          using (StreamWriter writer = new StreamWriter(stream))
          {
            writer.Write(dataToStore);
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogError("Error occurred when trying to save data file: " + fullPath + "\n" + e);
      }
    }
  }
}