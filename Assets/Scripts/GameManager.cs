using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    bool isInBattle = false;

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

    // Update is called once per frame
    void Update()
    {
        if (BattleManager.Instance.CurrentState == GameState.NotBattle && !isInBattle)
        {
            isInBattle = true;
            BattleManager.Instance.StartBattle();
        }
    }
}
