using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance{ get; private set; }
    public List<EventObject> eventObjects;
    public EventUI eventUI;

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
        
        yield return StartCoroutine(eventObject.Execute(this));
        
        GameManager.Instance.CurrentState = GameState.NotBattle;
        Debug.Log("Event Finished!");
    }
}