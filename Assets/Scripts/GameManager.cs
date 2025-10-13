using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    PlayerTurn,
    PlayerRunning,
    EnemyTurn,
    Paused
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; set; } = GameState.PlayerTurn;
    private readonly List<Rigidbody2D> activeRigidbodies = new();
    
    void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    public void StartEnemyTurn()
    {
        if (CurrentState == GameState.EnemyTurn) 
            return;
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitUntil(AllObjectsStopped);

        CurrentState = GameState.EnemyTurn;
        Debug.Log("Enemy Turn Started");
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.Act();
            yield return new WaitUntil(AllObjectsStopped);
        }
        Debug.Log("Enemy Turn Ended");

        yield return new WaitForSeconds(1f);
        CurrentState = GameState.PlayerTurn;
        Debug.Log("Player Turn Started");
    }
    
    private bool AllObjectsStopped()
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
}
