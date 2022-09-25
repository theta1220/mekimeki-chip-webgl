public class GameData : Singleton<GameData>
{
    public SaveData SaveData;

    public GameData()
    {
        SaveData = new SaveData();
    }
}