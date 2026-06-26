using UnityEngine;
using System.Collections;

public interface ISummonable 
{
    void Init(SummonData data);
    IEnumerator Execute();
}

public class SummonData
{
    
}