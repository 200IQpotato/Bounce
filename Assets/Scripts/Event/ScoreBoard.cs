using System;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public Action<int, bool> OnHit;
    private int scoreValue = 1;
    private bool isAdd = true;

    void Awake()
    {
        scoreValue = UnityEngine.Random.Range(1, 6);
        isAdd = UnityEngine.Random.value > 0.5f;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnHit?.Invoke(scoreValue, isAdd);
        }
    }
}
