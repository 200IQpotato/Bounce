using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public List<EventObject> eventObjects;

    void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartEvent()
    {
        if (eventObjects.Count == 0)
            return;

        EventObject eventObject = eventObjects[Random.Range(0, eventObjects.Count)];
        StartCoroutine(RunEvent(eventObject));
    }

    private IEnumerator RunEvent(EventObject eventObject)
    {
        Debug.Log($"Event Start: {eventObject.eventName}");

        GameManager.Instance.CurrentState = GameState.PlayerTurn;
        foreach (var step in eventObject.steps)
        {
            yield return StartCoroutine(step.Execute(this));
        }
        GameManager.Instance.CurrentState = GameState.NotBattle;
        Debug.Log("Event Finished!");
    }
}