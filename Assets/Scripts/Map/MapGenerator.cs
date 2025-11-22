using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NodeState{ Locked, Unlocked, Completed }

public enum NodeType { Battle, Elite, Event, Shop, Rest, Boss }

public class MapNode
{
    public NodeType type;
    public GameObject nodeObject;
    public List<MapNode> nextNodes;
    public NodeState nodeState;
    public Vector2 position;
    public MapNode(NodeType type)
    {
        this.type = type;
        nodeObject = null;
        nextNodes = new();
        nodeState = NodeState.Locked;
    }

    public MapNode(NodeType type, NodeState state)
    {
        this.type = type;
        nodeObject = null;
        nextNodes = new();
        nodeState = state;
    }
}

public class MapGenerator : MonoBehaviour
{
    [Header("Node Prefabs")]
    public GameObject battleNode;
    public GameObject eliteNode;
    public GameObject eventNode;
    public GameObject shopNode;
    public GameObject restNode;
    public GameObject bossNode;

    [Header("UI References")]
    public Transform mapContainer;  // 放置所有節點的容器
    public GameObject linePrefab;   // 連線的 Prefab (Image)
    public ScrollRect scrollRect;   // ScrollRect 組件

    [Header("Map Settings")]
    [SerializeField] private int levelHeight = 10;
    [SerializeField] private int levelMaxWidth = 6;
    [SerializeField] private int levelMinWidth = 4;
    [SerializeField] private int fixedNode = 4;

    [Header("Layout Settings")]
    [SerializeField] private int layerSpacing = 200;  // 層與層之間的垂直距離
    [SerializeField] private int nodeSpacing = 150;   // 同一層節點之間的水平距離
    [SerializeField] private int randomOffset = 30;   // 隨機偏移範圍
    [SerializeField] private int bottomPadding = 100;

    [Header("Node Chance Weights")]
    [SerializeField] private int battleWeight = 50;
    [SerializeField] private int eliteWeight = 15;
    [SerializeField] private int eventWeight = 15;
    [SerializeField] private int shopWeight = 10;
    [SerializeField] private int restWeight = 10;

    private List<List<MapNode>> map = new();
    private List<GameObject> lines = new();
    public static event System.Action<MapNode> OnNodeChosen;
    private int currentLayer = 0;

    void Awake()
    {
        
    }

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearMap();
        GenerateMapNodes();
        MapNodeConnect();
        CalculateNodePositions();
        InstantiateNodes();
        DrawLines();
        AdjustScrollView();
    }

    void ClearMap()
    {
        map.Clear();
        
        // 清除舊的節點
        foreach (Transform child in mapContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 清除舊的連線
        foreach (var line in lines)
        {
            Destroy(line);
        }
        lines.Clear();
    }

    void GenerateMapNodes()
    {
        for (int i = 0; i < levelHeight; i++)
        {
            List<MapNode> nodes = new();
            if (i == 0)
            {
                for (int j = 0; j < fixedNode; j++)
                {
                    nodes.Add(new MapNode(NodeType.Battle, NodeState.Unlocked));
                }
            }
            else if (i == 8)
            {
                for (int j = 0; j < fixedNode; j++)
                {
                    nodes.Add(new MapNode(NodeType.Rest));
                }
            }
            else if (i == 9)
            {
                nodes.Add(new MapNode(NodeType.Boss));
            }
            else
            {
                int nodeCount = Random.Range(levelMinWidth, levelMaxWidth+1);
                for (int j = 0; j < nodeCount; j++)
                {
                    NodeType type = GetRandomNodeWeighted();
                    nodes.Add(new MapNode(type));
                }
            }
            map.Add(nodes);
        }
    }

    void MapNodeConnect()
    {
        for (int layer = 0; layer < map.Count - 1; layer++)
        {
            List<MapNode> curr = map[layer];
            List<MapNode> next = map[layer + 1];

            int lastConnectedIndex = 0;

            for (int i = 0; i < curr.Count; i++)
            {
                MapNode node = curr[i];

                int currCount = curr.Count - i;
                int nextCount = next.Count - lastConnectedIndex;
                float widthDiff = currCount - nextCount;
                float chance = Mathf.Clamp(0.5f - (widthDiff * 0.15f), 0.1f, 0.9f);

                // 候選 index
                int idxA = lastConnectedIndex;
                int idxB = Mathf.Min(lastConnectedIndex + 1, next.Count - 1);
                int idxC = Mathf.Min(lastConnectedIndex + 2, next.Count - 1);

                // 先決定連幾條
                if ((float)nextCount / currCount >= 2)                
                    chance = 1f;                

                bool twoLines = Random.value < chance;
                int startFrom = (Random.value < chance) ? idxB : idxA;

                if (i == 0)
                    startFrom = idxA;


                if (!twoLines)
                {
                    node.nextNodes.Add(next[startFrom]);
                    lastConnectedIndex = startFrom;

                }
                else
                {
                    if (startFrom == idxA)
                    {
                        node.nextNodes.Add(next[idxA]);
                        if (idxB != idxA)
                            node.nextNodes.Add(next[idxB]);
                        lastConnectedIndex = idxB;
                    }
                    else
                    {
                        node.nextNodes.Add(next[idxB]);
                        if (idxB != idxC)
                            node.nextNodes.Add(next[idxC]);
                        lastConnectedIndex = idxC;
                    }
                }
            }
        }
    }
    
    // void CalculateNodePositions()
    // {
    //     Canvas canvas = GetComponentInChildren<Canvas>();
    //     int scaleFactor = canvas != null ? (int)canvas.scaleFactor : 1;

    //     for (int layer = 0; layer < map.Count; layer++)
    //     {
    //         List<MapNode> nodes = map[layer];
            
    //         // 修改處: yPos 加上 bottomPadding，確保第一層不會被切一半
    //         int yPos = bottomPadding + layer * layerSpacing * scaleFactor;

    //         // 計算該層的總寬度，讓節點居中
    //         int totalWidth = (nodes.Count - 1) * nodeSpacing * scaleFactor;
    //         int startX = (int)(-totalWidth / 2f);

    //         for (int i = 0; i < nodes.Count; i++)
    //         {
    //             int xPos = startX + i * nodeSpacing * scaleFactor;
    //             float currentYPos = yPos;

    //             // 添加隨機偏移（但第一層和最後一層不偏移）
    //             if (layer != 0 && layer != map.Count - 1)
    //             {
    //                 xPos += Random.Range(-randomOffset, randomOffset);
    //                 yPos += Random.Range((int)(-randomOffset * 0.5f), (int)(randomOffset * 0.5f));
    //             }

    //             nodes[i].position = new Vector2(xPos, yPos);
    //         }
    //     }
    // }

    void CalculateNodePositions()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;

        for (int layer = 0; layer < map.Count; layer++)
        {
            List<MapNode> nodes = map[layer];
            
            float yPos = AlignValue(bottomPadding, scaleFactor) + layer * AlignValue(layerSpacing, scaleFactor);

            float alignedNodeSpacing = AlignValue(nodeSpacing, scaleFactor);
            float totalWidth = (nodes.Count - 1) * alignedNodeSpacing;
            float startX = AlignValue(-totalWidth / 2f, scaleFactor); 

            for (int i = 0; i < nodes.Count; i++)
            {
                float xPos = startX + i * alignedNodeSpacing;
                float currentYPos = yPos;

                if (layer != 0 && layer != map.Count - 1)
                {
                    xPos += GetAlignedRandomOffset(-randomOffset, randomOffset, scaleFactor);
                    currentYPos += GetAlignedRandomOffset((int)(-randomOffset * 0.5f), (int)(randomOffset * 0.5f), scaleFactor);
                }

                nodes[i].position = new Vector2(xPos, currentYPos);
            }
        }
    }

    // ✨ 新增：對齊單一數值到像素網格
    float AlignValue(float value, float scale)
    {
        return Mathf.Round(value / scale) * scale;
    }

    // ✨ 新增：產生對齊的隨機偏移
    float GetAlignedRandomOffset(float min, float max, float scale)
    {
        // 將範圍轉換為網格單位
        int gridMin = Mathf.CeilToInt(min / scale);
        int gridMax = Mathf.FloorToInt(max / scale);
        
        // 在網格上隨機選擇
        int gridOffset = Random.Range(gridMin, gridMax + 1);
        
        // 轉換回實際座標
        return gridOffset * scale;
    }

    void InstantiateNodes()
    {
        foreach (var layer in map)
        {
            foreach (var node in layer)
            {
                GameObject prefab = GetNodePrefab(node.type);
                if (prefab != null)
                {
                    GameObject nodeObj = Instantiate(prefab, mapContainer);
                    nodeObj.GetComponent<RectTransform>().anchoredPosition = node.position;
                    node.nodeObject = nodeObj;

                    // 可以在這裡設置節點的點擊事件
                    Button btn = nodeObj.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.AddListener(() => OnNodeClicked(node));
                    }
                }
            }
        }
    }

    void DrawLines()
    {
        if (linePrefab == null) return;

        foreach (var layer in map)
        {
            foreach (var node in layer)
            {
                foreach (var nextNode in node.nextNodes)
                {
                    GameObject line = Instantiate(linePrefab, mapContainer);
                    line.transform.SetAsFirstSibling();  // 讓連線在節點下方

                    RectTransform lineRect = line.GetComponent<RectTransform>();
                    Image lineImage = line.GetComponent<Image>();

                    // 計算起點和終點
                    Vector2 start = node.position;
                    Vector2 end = nextNode.position;

                    // 設置連線位置和旋轉
                    Vector2 direction = end - start;
                    float distance = direction.magnitude;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    lineRect.anchoredPosition = start;
                    lineRect.sizeDelta = new Vector2(distance, 5f);  // 寬度為距離，高度為線條粗細
                    lineRect.pivot = new Vector2(0, 0.5f);
                    lineRect.rotation = Quaternion.Euler(0, 0, angle);

                    lines.Add(line);
                }
            }
        }
    }

    void AdjustScrollView()
    {
        if (scrollRect == null) return;

        RectTransform content = mapContainer.GetComponent<RectTransform>();

        float maxNodeY = 0f;
        if (map.Count > 0)
        {
            MapNode lastNode = map[map.Count - 1][0];
            maxNodeY = lastNode.position.y;
        }

        Canvas canvas = GetComponentInChildren<Canvas>();
        float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
        float finalHeight = maxNodeY + AlignValue(bottomPadding, scaleFactor); 

        
        content.sizeDelta = new Vector2(content.sizeDelta.x, finalHeight);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // 0 代表回到最底部
    }

    GameObject GetNodePrefab( NodeType nodeType )
    {
        switch (nodeType)
        {
            case NodeType.Battle:
                return battleNode;
            
            case NodeType.Elite:
                return eliteNode;

            case NodeType.Event:
                return eventNode;

            case NodeType.Shop:
                return shopNode;

            case NodeType.Rest:
                return restNode;

            case NodeType.Boss:
                return bossNode;

            default:
                return null;
        }
    }

    void OnNodeClicked(MapNode node)
    {
        Debug.Log($"Clicked on {node.type} node now state {node.nodeState}");
        
        if ( BattleManager.Instance.CurrentState != GameState.NotBattle )
        {
            Debug.Log("Cannot choose node during battle.");
            return;
        }
        else if ( node.nodeState == NodeState.Locked || node.nodeState == NodeState.Completed )
        {
            Debug.Log("This node is locked.");
            return;
        }
        else
        {
            OnNodeChosen?.Invoke( node );
            LockLayerNodes( currentLayer++ );
            node.nodeState = NodeState.Completed;
            for ( int i = 0; i < node.nextNodes.Count; i++ )
            {
                UnlockNode( node.nextNodes[i] );
            }
        }
    }

    public void UnlockNode( MapNode node )
    {
        node.nodeState = NodeState.Unlocked;
        // 可以在這裡更改節點的視覺效果，例如改變顏色或圖片
    }

    public void LockLayerNodes( int layer )
    {
        if ( layer < 0 || layer >= map.Count )
            return;

        for ( int i = 0; i < map[layer].Count; i++ )
        {
            MapNode node = map[layer][i];
            node.nodeState = NodeState.Locked;
            // 可以在這裡更改節點的視覺效果，例如改變顏色或圖片
        }
    }

    NodeType GetRandomNodeWeighted()
    {
        Dictionary<NodeType, int> weights = new Dictionary<NodeType, int>()
        {
            { NodeType.Battle, battleWeight },
            { NodeType.Event, eventWeight },
            { NodeType.Shop, shopWeight },
            { NodeType.Rest, restWeight },
            { NodeType.Elite, eliteWeight }
        };

        int total = 0;
        foreach (var w in weights.Values)
            total += w;

        int r = Random.Range(0, total);
        int sum = 0;

        foreach (var kv in weights)
        {
            sum += kv.Value;
            if (r < sum)
                return kv.Key;
        }

        return NodeType.Battle; // 安全保底
    }
}
