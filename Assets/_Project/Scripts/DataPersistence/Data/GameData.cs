namespace TwelveG.SaveSystem
{
  [System.Serializable]
  public class GameData
  {
    // Scene data
    public int savedSceneIndex;
    public int savesNumber;

    // Localization
    public string languageCode;

    // Audio data
    public float masterVolume;
    public float interfaceVolume;
    public float musicVolume;
    public float sfxVolume;

    // Valores inciales para cuando decidimos iniciar un nuevo juego
    public GameData()
    {
      this.savedSceneIndex = 0;
      this.savesNumber = 0;

      this.languageCode = "en";
    }
  }
}