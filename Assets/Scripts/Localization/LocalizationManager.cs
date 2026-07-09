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
    public static event System.Action OnLanguageChanged;
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
        string path = string.Empty;
        switch (lang)
        {
            case Language.English:
                path = "Assets/Scripts/Localization/English/En.json";
                break;
            case Language.Chinese:
                path = "Assets/Scripts/Localization/Chinese/Ch.json";
                break;
            default:
                Debug.LogWarning($"Unsupported language: {lang}");
                break;
        }
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
        Debug.Log($"Setting language to: {lang}");
        CurrentLanguage = lang;
        LoadLanguage(lang);
        OnLanguageChanged?.Invoke();
    }

    public void SetLanguageByIndex(int index)
    {
        if (index < 0 || index >= System.Enum.GetValues(typeof(Language)).Length)
        {
            Debug.LogWarning($"Invalid language index: {index}");
            return;
        }
        SetLanguage((Language)index);
    }

    public DescriptionData ResolveLink(string linkId) //傳入 "Relics/RelicID" 或 "Effects/EffectID" 或 "Glossary/GlossaryID" 這種格式的字串, 回傳對應的 DescriptionData
    {
        var parts = linkId.Split('/');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"Malformed link id: {linkId}");
            return new DescriptionData { name = "Unknown", description = "Unknown ID" };
        }

        string category = parts[0];
        string id = parts[1];
        Debug.Log($"Resolving link: Category={category}, ID={id}");

        switch (category)
        {
            case "Relics":
                return GetRelicData(id);
            case "Effects":
                return GetEffectData(id);
            case "Glossary":
                return GetGlossaryData(id);
            default:
                Debug.LogWarning($"Unknown link category: {category}");
                return new DescriptionData { name = "Unknown", description = "Unknown ID" };
        }
    }

    public DescriptionData GetRelicData(string id) => relics.ContainsKey(id) ? relics[id] : new DescriptionData { name = "Unknown Relic", description = "Unknown Relic Description" };
    public DescriptionData GetEffectData(string id) => effects.ContainsKey(id) ? effects[id] : new DescriptionData { name = "Unknown Effect", description = "Unknown Effect Description" };
    public DescriptionData GetGlossaryData(string id) => glossary.ContainsKey(id) ? glossary[id] : new DescriptionData { name = "Unknown Glossary Entry", description = "Unknown Glossary Entry Description" };
    public DescriptionData Get(string id)
    {
        if (relics.ContainsKey(id)) return relics[id];
        if (effects.ContainsKey(id)) return effects[id];
        if (glossary.ContainsKey(id)) return glossary[id];

        Debug.LogWarning($"ID not found in any category: {id}");
        return new DescriptionData { name = "Unknown", description = "Unknown ID" };
    }
    public string GetRelicName(string id) => relics.ContainsKey(id) ? relics[id].name : "Unknown Relic";
    public string GetRelicDescription(string id) => relics.ContainsKey(id) ? relics[id].description : "Unknown Relic Description";
    public string GetEffectName(string id) => effects.ContainsKey(id) ? effects[id].name : "Unknown Effect";
    public string GetEffectDescription(string id) => effects.ContainsKey(id) ? effects[id].description : "Unknown Effect Description";
    public string GetGlossaryName(string id) => glossary.ContainsKey(id) ? glossary[id].name : "Unknown Glossary Entry";
    public string GetGlossaryDescription(string id) => glossary.ContainsKey(id) ? glossary[id].description : "Unknown Glossary Entry Description";
}
