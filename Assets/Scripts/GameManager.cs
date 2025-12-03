using System.Collections.Generic;
using UnityEngine;

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
    public List<BattleLevelSO> battleLevels;
    public GameObject playerPrefab;
    [SerializeField] private Vector2 playerSpawnPoint;
    public Player playerInstance;

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

    void Start()
    {
        var player = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        playerInstance = player.GetComponent<Player>();
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
        switch ( nodeType )
        {
            case NodeType.Battle:
                Debug.Log("Creating Combat Level");
                int level = Random.Range(0, battleLevels.Count);
                InstantiateBattleLevel( battleLevels[level] );
                break;

            case NodeType.Elite:
                Debug.Log("Creating Elite Level");
                break;

            case NodeType.Event:
                Debug.Log("Creating Event Level");
                EventManager.Instance.StartEvent();
                break;

            case NodeType.Shop:
                Debug.Log("Creating Shop Level");
                break;

            case NodeType.Rest:
                Debug.Log("Creating Rest Level");
                break;

            case NodeType.Boss:
                Debug.Log("Creating Boss Level");
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
        BattleManager.Instance.StartBattle();
    }
}
