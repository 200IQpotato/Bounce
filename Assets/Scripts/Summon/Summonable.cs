using UnityEngine;
using System.Collections;

public class SummonData
{
    public int damage;
    public IBattleEntity caster;

    public SummonData(int damage, IBattleEntity caster)
    {
        this.damage = damage;
        this.caster = caster;
    }

    public SummonData Clone()
    {
        return new SummonData(damage, caster);
    }
}

public abstract class Summonable : MonoBehaviour
{
    protected SummonData data;

    void OnEnable()
    {
        BattleManager.Instance.RegisterSummon(this);
    }

    public void Init(SummonData summonData)
    {
        data = summonData;
        StartCoroutine(RunAndCleanup());
    }

    private IEnumerator RunAndCleanup()
    {
        yield return StartCoroutine(Execute());
        BattleManager.Instance.UnregisterSummon(this);
        Destroy(gameObject);
    }

    protected abstract IEnumerator Execute();
}