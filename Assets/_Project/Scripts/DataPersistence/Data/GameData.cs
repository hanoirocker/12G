namespace TwelveG.SaveSystem
{
  [System.Serializable]
  public class GameData
  {
    public int sceneIndex;
    public int eventIndex;

    public int savesNumber;

    public string languageCode;

    // Valores inciales para cuando decidimos iniciar un nuevo juego
    public GameData()
    {
      this.sceneIndex = 0;
      this.eventIndex = 0;

      this.savesNumber = 0;

      this.languageCode = "en";
    }
  }
}