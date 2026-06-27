using UnityEngine;
using System.Collections;

public class FireBall : Summonable
{
    protected override IEnumerator Execute()
    {
        Enemy targetEnemy = null;
        foreach ( Enemy enemy in BattleManager.Instance.GetEnemies())
        {
            if (enemy != null)
            {
                if (targetEnemy == null || Vector2.Distance(transform.position, enemy.transform.position) < Vector2.Distance(transform.position, targetEnemy.transform.position))
                {
                    targetEnemy = enemy;
                }
            }
        }
        if (targetEnemy != null)
        {
            while (Vector2.Distance(transform.position, targetEnemy.transform.position) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetEnemy.transform.position, 5f * Time.deltaTime);
                yield return null;
            }
            targetEnemy.TakeDamage(data.damage, DamageType.Hit);
        }
        yield break;
    }
}
