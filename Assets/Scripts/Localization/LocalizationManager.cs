using UnityEngine;
using System.Collections.Generic;
using System.IO;

public enum Language
{
    English,
    Chinese
}

[System.Serializable]
public class DataLocEntry { public string id; public string name; public string description; }
[System.Serializable]
public class DescriptionData { public string name; public string description; }

[System.Serializable]
public class LocFile
{
    public List<DataLocEntry> Relics;
    public List<DataLocEntry> Effects;
    public List<DataLocEntry> Glossary;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public Language CurrentLanguage { get; private set; } = Language.English;
    private Dictionary<string, DescriptionData> relics = new Dictionary<string, DescriptionData>();
    private Dictionary<string, DescriptionData> effects = new Dictionary<string, DescriptionData>();
    private Dictionary<string, DescriptionData> glossary = new Dictionary<string, DescriptionData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLanguage(CurrentLanguage);
    }

    public void LoadLanguage(Language lang)
    {
        string path = lang == Language.Chinese ? "Assets/Scripts/Localization/Chinese/Zh.json" : "Assets/Scripts/Localization/English/En.json";
        LocFile data = JsonUtility.FromJson<LocFile>(File.ReadAllText(path));

        relics.Clear();
        effects.Clear();
        glossary.Clear();

        foreach (var r in data.Relics) relics[r.id] = new DescriptionData { name = r.name, description = r.description };
        foreach (var e in data.Effects) effects[e.id] = new DescriptionData { name = e.name, description = e.description };
        foreach (var g in data.Glossary) glossary[g.id] = new DescriptionData { name = g.name, description = g.description };
    }

    public void SetLanguage(Language lang)
    {
        CurrentLanguage = lang;
        LoadLanguage(lang);
    }

    public string GetRelicName(string id) => relics.ContainsKey(id) ? relics[id].name : "Unknown Relic";
    public string GetRelicDescription(string id) => relics.ContainsKey(id) ? relics[id].description : "Unknown Relic Description";
    public string GetEffectName(string id) => effects.ContainsKey(id) ? effects[id].name : "Unknown Effect";
    public string GetEffectDescription(string id) => effects.ContainsKey(id) ? effects[id].description : "Unknown Effect Description";
    public string GetGlossaryName(string id) => glossary.ContainsKey(id) ? glossary[id].name : "Unknown Glossary Entry";
    public string GetGlossaryDescription(string id) => glossary.ContainsKey(id) ? glossary[id].description : "Unknown Glossary Entry Description";
}
