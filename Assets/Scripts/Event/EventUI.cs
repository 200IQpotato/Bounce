using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class EventChoice
{
    public string choiceText;
    public int choiceID;
    public bool isAvailable;
    public string lockText;

    public EventChoice( string choiceText, int choiceID, bool isAvailable )
    {
        this.choiceText = choiceText;
        this.choiceID = choiceID;
        this.isAvailable = isAvailable;
    }
}

public class EventUI : MonoBehaviour
{
    private int selectedChoice = -1;
    [SerializeField] private Text description;
    [SerializeField] private Image image;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ChoiceText choicePrefab;
    private List<ChoiceText> prefabs = new List<ChoiceText>();

    public IEnumerator ShowChoices(string description, Sprite image, List<EventChoice> choices)
    {
        Init();
        gameObject.SetActive(true);        

        this.description.text = description;
        this.image.sprite = image;

        CreateButtons(choices);

        // 等玩家點選
        yield return new WaitUntil(() => selectedChoice != -1);

        gameObject.SetActive(false);
    }

    public int GetResult()
    {
        return selectedChoice;
    }

    private void OnChoiceClicked(int id)
    {
        selectedChoice = id;
    }

    private void CreateButtons(List<EventChoice> choices)
    {
        foreach (EventChoice choice in choices)
        {
            CreateButton(choice);
        }
    }

    private void CreateButton(EventChoice choice)
    {
        ChoiceText choiceText = Instantiate(choicePrefab, contentTransform);
        prefabs.Add(choiceText);
        choiceText.Init(choice);
        if(choice.isAvailable)
            choiceText.GetComponent<Button>().onClick.AddListener(() => OnChoiceClicked(choice.choiceID));
    }

    public void Init()
    {
        selectedChoice = -1;
        foreach ( ChoiceText choice in prefabs)
        {
            Destroy(choice.gameObject);
        }
        prefabs.Clear();
    }
}
