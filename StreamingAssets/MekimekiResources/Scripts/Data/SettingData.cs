using System;

public enum LanguageType
{
    Ja,
    En,
}

[Serializable]
public class SettingData
{
    public LanguageType LanguageType { get; set; }
}