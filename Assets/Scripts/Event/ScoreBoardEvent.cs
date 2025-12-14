using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ScoreBoardEvent", menuName = "Event/ScoreBoardEvent")]
public class ScoreBoardEvent : EventObject
{
    public GameObject scoreBoardPrefab;
    public Sprite image;
    public int targetScore;
    public int boardCount;
    public Vector2 rangeMin;
    public Vector2 rangeMax;
    private int score;
    private int shootCount = 3;
    private List<GameObject> spawnedBoards = new List<GameObject>();
    public List<EventChoice> choicies;
    

    public override IEnumerator Execute(EventManager manager)
    {
        score = 0;

        yield return manager.eventUI.ShowChoices("You see a strange scoreboard...", image, choicies);

        int result = manager.eventUI.GetResult();

        if (result == 1)
        {
            Debug.Log("player ignore");
            yield break;
        }
            

        for (int i = 0; i < boardCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(rangeMin.x, rangeMax.x),
                Random.Range(rangeMin.y, rangeMax.y)
            );

            var obj = Instantiate(scoreBoardPrefab, pos, Quaternion.identity);
            obj.GetComponent<ScoreBoard>().OnHit += AddScore;
            spawnedBoards.Add(obj);
        }

        int j = 0;
        while (j < shootCount){
            yield return manager.StartCoroutine(GameManager.Instance.playerInstance.TakeTurn());
            j++;
        }

        foreach (var board in spawnedBoards)
        {
            Destroy(board);
        }

        if (score >= targetScore)
        {
            Debug.Log("ScoreBoard Event Succeeded!");
        }
        else
        {
            Debug.Log("ScoreBoard Event Failed!");
        }
    }

    private void AddScore(int value, bool isAdd)
    {
        if (isAdd)
            score += value;
        else
            score *= value;
    }
}
