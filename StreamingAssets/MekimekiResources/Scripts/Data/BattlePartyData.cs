using System;
using System.Collections.Generic;

[Serializable]
public class BattlePartyData
{
    public List<BattleCharacterData> BattleCharacterDatas;

    public BattlePartyData()
    {
        BattleCharacterDatas = new List<BattleCharacterData>();
    }
}