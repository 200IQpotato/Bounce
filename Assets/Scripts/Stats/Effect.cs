using UnityEngine;

public class Effect
{
    public EffectObject effectObject;
    public int duration;
    public int stackCount;
    public bool isConsumed;

    public Effect(EffectObject effectObject, int duration)
    {
        this.effectObject = effectObject;
        this.duration = duration;
        this.stackCount = 1;
        this.isConsumed = false;
    }

    public Effect(EffectObject effectObject, int duration, int stackCount)
    {
        this.effectObject = effectObject;
        this.duration = duration;
        this.stackCount = stackCount;
        this.isConsumed = false;
    }
}
