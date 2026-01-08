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
    public List<EventChoice> choices;
    

    public override IEnumerator Execute(EventManager manager)
    {
        score = 0;

        yield return manager.eventUI.ShowChoices("You see a strange scoreboard...", image, choices);

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

            Quaternion rot = Quaternion.Euler( 0, 0, Random.Range(0, 360));

            var obj = Instantiate(scoreBoardPrefab, pos, rot);
            obj.GetComponent<ScoreBoard>().OnHit += AddScore;
            spawnedBoards.Add(obj);
        }
        EventTextUI.Instance.Show("Current Score: " + score.ToString());

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
            EventTextUI.Instance.Show("You Success!\nMax Health +3");
            GameManager.Instance.playerInstance.stats.ModifyMaxHealth(3);
            GameManager.Instance.playerInstance.stats.Heal(3);
        }
        else
        {
            EventTextUI.Instance.Show("You Failed!");
            GameManager.Instance.playerInstance.TakeDamage(15);
        }

        yield return new WaitForSeconds(2f);
        EventTextUI.Instance.Hide();
    }

    private void AddScore(int value, bool isAdd)
    {
        if (isAdd)
            score += value;
        else
            score *= value;

        EventTextUI.Instance.Show("Current Score: " + score.ToString());
    }
}
