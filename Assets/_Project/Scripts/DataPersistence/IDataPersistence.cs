namespace TwelveG.SaveSystem
{
  public interface IDataPersistence
  {
    // Solo lectura
    void LoadData(GameData data);

    // Permitir modificar el objeto por referencia
    void SaveData(ref GameData data);
  }
}