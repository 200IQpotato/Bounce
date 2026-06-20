using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    private readonly List<Rigidbody2D> activeRigidbodies = new();
    private readonly List<Enemy> enemies = new();
    private readonly List<Player> players = new();
    private readonly List<IBattleEntity> entities = new();
    public event Func<int, bool, IEnumerator> OnTurnStartEndUI;
    public event Func<IEnumerator> OnBattleEnd;
    private int turnCount = 0;
    
    void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }

    public void RegisterRigidbody(Rigidbody2D rb)
    {
        if (rb != null && !activeRigidbodies.Contains(rb))
            activeRigidbodies.Add(rb);
    }

    public void UnregisterRigidbody(Rigidbody2D rb)
    {
        if (rb != null)
            activeRigidbodies.Remove(rb);
    }
    
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemy != null)
            enemies.Remove(enemy);
    }

    public void RegisterPlayer(Player player)
    {
        if (player != null && !players.Contains(player))
            players.Add(player);
    }

    public void UnregisterPlayer(Player player)
    {
        if (player != null)
            players.Remove(player);
    }

    public void RegisterEntity(IBattleEntity entity)
    {
        if (entity != null && !entities.Contains(entity))
            entities.Add(entity);
    }

    public void UnregisterEntity(IBattleEntity entity)
    {
        if (entity != null)
            entities.Remove(entity);
    }

    public void StartBattle()
    {
        Debug.Log("Battle Start");
        turnCount = 0;
        RelicManager.Instance.OnCountInit();
        foreach ( Player player in players )
        {
            player.stats.CleanEffect();
        }
        StartCoroutine(GameLoop());
        RelicManager.Instance.OnCountInit();
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            turnCount++;
            yield return StartCoroutine(OnTurnStartEndUI?.Invoke(turnCount, true));

            foreach (IBattleEntity entity in entities)
            {
                entity.OnTurnStart();
            }
            Debug.Log("New Turn");

            GameManager.Instance.CurrentState = GameState.PlayerTurn;
            foreach (ITurnBase player in players)
            {
                yield return StartCoroutine(player.TakeTurn());
            }

            GameManager.Instance.CurrentState = GameState.EnemyTurn;
            var enemiesCopy = new List<Enemy>(enemies);
            foreach (var enemy in enemiesCopy)
            {
                if (enemies.Contains(enemy))
                    yield return StartCoroutine(enemy.TakeTurn());
            }

            foreach (IBattleEntity entity in entities)
            {
                entity.OnTurnEnd();
            }

            Debug.Log("End Turn");
            yield return StartCoroutine(OnTurnStartEndUI?.Invoke(turnCount, false));

            if (enemies.Count == 0)
            {
                if (OnBattleEnd != null)
                    yield return StartCoroutine(OnBattleEnd.Invoke());
                    
                GameManager.Instance.CurrentState = GameState.NotBattle;
                DestroyObstacles();
                Debug.Log("Battle End");
                yield break;
            }
        }
    }
    
    public bool AllObjectsStopped()
    {
        foreach (var rb in activeRigidbodies)
        {
            if (rb == null) continue;
            if (rb.linearVelocity.magnitude != 0)
                return false;
        }
        Debug.Log("All objects have stopped.");
        return true;
    }

    private void DestroyObstacles()
    {
        List<Obstacle> toRemove = new List<Obstacle>();
        foreach (var entity in entities)
        {
            if (entity is Obstacle obstacle)
            {
                toRemove.Add(obstacle);
            }
        }

        foreach (var entity in toRemove)
        {
            UnregisterEntity(entity);
            Destroy(entity.gameObject);
        }
    }
}
