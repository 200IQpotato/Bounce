using UnityEngine;
using System.Collections;

public abstract class EventStep : ScriptableObject
{
    public abstract IEnumerator Execute(EventManager manager);
}