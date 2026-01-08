using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public Action<int, bool> OnHit;
    private int scoreValue = 1;
    private bool isAdd = true;
    [SerializeField] private Text scoreText;

    void Awake()
    {
        scoreValue = UnityEngine.Random.Range(1, 6);        
        scoreText.rectTransform.position = Camera.main.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y));
        isAdd = UnityEngine.Random.value > 0.5f;
        if( isAdd )
            scoreText.text = "+" + scoreValue.ToString();
        else
            scoreText.text = "x" + scoreValue.ToString();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnHit?.Invoke(scoreValue, isAdd);
        }
    }
}
