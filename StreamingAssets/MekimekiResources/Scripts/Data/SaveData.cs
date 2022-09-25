using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int MapId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public BattlePartyData BattlePartyData { get; set; }
    public List<int> EventFlags { get; set; }

    public SaveData()
    {
        BattlePartyData = new BattlePartyData()
        {
            BattleCharacterDatas = new List<BattleCharacterData>
            {
                new BattleCharacterData
                {
                    CurrentHp = 100,
                    ResourceId = 1,
                }
            }
        };
        EventFlags = new List<int>();

        MapId = 1;
    }
}