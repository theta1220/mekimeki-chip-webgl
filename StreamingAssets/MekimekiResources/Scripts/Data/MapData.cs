
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MapData
    {
        public int ResourceId { get; set; }
        public List<CharacterData> Characters { get; set; }
        public List<SymbolData> Symbols { get; set; }

        public MapData()
        {
            Characters = new List<CharacterData>();
            Symbols = new List<SymbolData>();
        }

        public void OnDeserialized()
        {
            if (Characters == null) Characters = new List<CharacterData>();
            if (Symbols == null) Symbols = new List<SymbolData>();
        }
        
    }
