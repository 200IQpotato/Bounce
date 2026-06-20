using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    PlayerTurn,
    EnemyTurn,
    NotBattle,
    Battling
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; set; } = GameState.NotBattle;
    public BattleLevelSO currentBattleLevel;
    public List<BattleLevelSO> battleLevels;
    public List<LevelObstacleSO> levelObstacles;
    public GameObject playerPrefab;
    [SerializeField] private Vector2 playerSpawnPoint;
    public Player playerInstance;
    public bool isUIBlockingInput = false;
    public System.Action<Player> OnPlayerSpawned;
    public System.Action<List<RelicObject>, int, System.Action> RelicChooseEvent;

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

    void Start()
    {
        SpawnPlayer();
        BattleManager.Instance.OnBattleEnd += GetReward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        MapGenerator.OnNodeChosen += HandleNodeChosen;
    }

    void OnDisable()
    {
        MapGenerator.OnNodeChosen -= HandleNodeChosen;
    }

    void SpawnPlayer()
    {
        var go = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        playerInstance = go.GetComponent<Player>();
        OnPlayerSpawned?.Invoke(playerInstance);
    }

    void HandleNodeChosen( MapNode node )
    {
        CreateLevel( node.type );
    }

    public void CreateLevel( NodeType nodeType )
    {
        if( CurrentState != GameState.NotBattle )
            return;

        playerInstance.transform.position = playerSpawnPoint;
        Instance.CurrentState = GameState.Battling;
        int level = Random.Range(0, battleLevels.Count);
        switch ( nodeType )
        {
            case NodeType.Battle:
                Debug.Log("Creating Combat Level");
                currentBattleLevel = battleLevels[level];
                InstantiateBattleLevel( currentBattleLevel );
                break;

            case NodeType.Elite:
                Debug.Log("Creating Elite Level");
                currentBattleLevel = battleLevels[level];
                InstantiateBattleLevel( currentBattleLevel );
                break;

            case NodeType.Event:
                Debug.Log("Creating Event Level");
                EventManager.Instance.StartEvent();
                break;

            case NodeType.Shop:
                Debug.Log("Creating Shop Level");
                EventManager.Instance.StartEvent(EventManager.Instance.GetShopEvent());
                break;

            case NodeType.Rest:
                Debug.Log("Creating Rest Level");
                EventManager.Instance.StartEvent(EventManager.Instance.GetRestEvent());
                break;

            case NodeType.Boss:
                Debug.Log("Creating Boss Level");
                currentBattleLevel = battleLevels[level];
                InstantiateBattleLevel( currentBattleLevel );
                break;

            default:
                Debug.Log("Unknown Node Type");
                break;
        }
    }

    void InstantiateBattleLevel( BattleLevelSO battleLevelSO )
    {
        int i = 0;
        foreach ( Enemy enemy in battleLevelSO.enemies )
        {
            Instantiate( enemy, battleLevelSO.spawnPoints[i], Quaternion.identity );
            i++;
        }
        i = 0;
        LevelObstacleSO obstacleSO = levelObstacles[Random.Range(0, levelObstacles.Count)];
        foreach ( Obstacle obstacle in obstacleSO.obstacles )
        {
            Instantiate( obstacle, obstacleSO.spawnPoints[i], obstacleSO.spawnRotations[i] ).transform.localScale 
            = new Vector3(obstacleSO.spawnScales[i].x, obstacleSO.spawnScales[i].y, 1);
            i++;
        }
        BattleManager.Instance.StartBattle();
    }

    public IEnumerator GetReward()
    {
        int moneyReward = Random.Range( currentBattleLevel.minMoneyReward, currentBattleLevel.maxMoneyReward );
        playerInstance.stats.ModifyMoney( moneyReward );

        bool relicChosen = false;
        RelicChooseEvent?.Invoke( currentBattleLevel.relicRewards, currentBattleLevel.relicCount, () => relicChosen = true );
        yield return new WaitUntil(() => relicChosen);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDestroy()
    {
        BattleManager.Instance.OnBattleEnd -= GetReward;
    }
}
