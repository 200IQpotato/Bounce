using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTurnEnd();
    bool isExpired{ get; }
}
