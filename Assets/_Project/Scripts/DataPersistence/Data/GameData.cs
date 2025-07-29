namespace TwelveG.SaveSystem
{
  [System.Serializable]
  public class GameData
  {
    public int SceneIndex;
    public int eventIndex;

    public string languageCode;

    // Valores inciales para cuando decidimos iniciar un nuevo juego
    public GameData()
    {
      this.SceneIndex = 0;
      this.eventIndex = 0;

      this.languageCode = "en";
    }
  }
}