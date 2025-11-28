using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Event/Event")]
public class EventObject : ScriptableObject
{
    public string eventName;
    public List<EventStep> steps;
}