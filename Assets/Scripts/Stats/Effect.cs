using UnityEngine;

public class Effect
{
    public EffectObject effectObject;
    public int duration;
    public int stackCount;

    public Effect(EffectObject effectObject, int duration)
    {
        this.effectObject = effectObject;
        this.duration = duration;
        this.stackCount = 1;
    }
}
