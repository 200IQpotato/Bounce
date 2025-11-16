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
    [SerializeField] private float layerSpacing = 200f;  // 層與層之間的垂直距離
    [SerializeField] private float nodeSpacing = 150f;   // 同一層節點之間的水平距離
    [SerializeField] private float randomOffset = 30f;   // 隨機偏移範圍

    private List<List<MapNode>> map = new();
    private List<GameObject> lines = new();
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
                    nodes.Add(new MapNode(NodeType.Battle));
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
                    NodeType type = (NodeType)Random.Range((int)NodeType.Battle, (int)NodeType.Rest+1);
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
                {
                    chance = 1f;
                    Debug.Log("chance = 1");
                }

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
    
    void CalculateNodePositions()
    {
        for (int layer = 0; layer < map.Count; layer++)
        {
            List<MapNode> nodes = map[layer];
            float yPos = layer * layerSpacing;

            // 計算該層的總寬度，讓節點居中
            float totalWidth = (nodes.Count - 1) * nodeSpacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < nodes.Count; i++)
            {
                float xPos = startX + i * nodeSpacing;

                // 添加隨機偏移（但第一層和最後一層不偏移）
                if (layer != 0 && layer != map.Count - 1)
                {
                    xPos += Random.Range(-randomOffset, randomOffset);
                    yPos += Random.Range(-randomOffset * 0.5f, randomOffset * 0.5f);
                }

                nodes[i].position = new Vector2(xPos, yPos);
            }
        }
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
                        MapNode capturedNode = node;  // 捕獲當前節點
                        btn.onClick.AddListener(() => OnNodeClicked(capturedNode));
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

        // 設置 Content 的大小（加大上下邊距）
        RectTransform content = mapContainer.GetComponent<RectTransform>();
        float height = (levelHeight - 1) * layerSpacing + 400f;  // 上下各多 200 邊距
        content.sizeDelta = new Vector2(content.sizeDelta.x, height);

        // 將滾動視圖移到底部（起始位置，從第一層開始）
        scrollRect.verticalNormalizedPosition = 0f;
        
        // 強制更新 Layout
        Canvas.ForceUpdateCanvases();
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
        Debug.Log($"Clicked on {node.type} node at position {node.position}");
        // 在這裡處理節點點擊邏輯
    }
}
