using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Event/Event")]
public abstract class EventObject : ScriptableObject
{
    public string eventName;
    public abstract IEnumerator Execute(EventManager manager);
}