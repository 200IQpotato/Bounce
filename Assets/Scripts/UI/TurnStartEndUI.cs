using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnStartEndUI : MonoBehaviour
{
    [SerializeField] private Text turnUI;
    [SerializeField] private CanvasGroup turnGroup;

    void Start()
    {
        BattleManager.Instance.OnTurnStartEndUI += TurnShow;
    }

    private IEnumerator TurnShow(int turnCount, bool isStart)
    {
        turnUI.text = isStart ? "Turn " + turnCount + " Start" : "Turn " + turnCount + " End";
        turnUI.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(turnGroup, 0f, 1f, 0.3f));

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeCanvasGroup(turnGroup, 1f, 0f, 0.3f));

        turnUI.gameObject.SetActive(false);
        yield break;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        cg.alpha = to;
    }
}
