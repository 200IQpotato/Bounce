using System.Collections.Generic;
using UnityEngine;

public enum NodeState{ Locked, Unlocked, Completed }

public enum NodeType { Battle, Elite, Event, Shop, Rest, Boss }

public class MapNode
{
    NodeType type;
    GameObject nodePrefab;
    public List<MapNode> nextNodes;
    NodeState nodeState;
    public MapNode(NodeType type)
    {
        this.type = type;
        nodePrefab = null;
        nextNodes = new();
        nodeState = NodeState.Locked;
    }
}

public class MapGenerator : MonoBehaviour
{
    public GameObject battleNode;
    public GameObject EliteNode;
    public GameObject EventNode;
    public GameObject ShopNode;
    public GameObject RestNode;
    public GameObject BossNode;
    private List<List<MapNode>> map = new();
    [SerializeField] private int levelHeight = 10;
    [SerializeField] private int levelMaxWidth = 6;
    [SerializeField] private int levelMinWidth = 4;
    [SerializeField] private int fixedNode = 4;

    void Start()
    {

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
                int nodeCount = Random.Range(levelMinWidth, levelMaxWidth);
                for (int j = 0; j < nodeCount; j++)
                {
                    NodeType type = (NodeType)Random.Range((int)NodeType.Battle, (int)NodeType.Rest);
                    nodes.Add(new MapNode(type));
                }
            }
            map.Add(nodes);
        }
    }

    void MapNodeConnect()
    {
        
    }
    
    GameObject GetNodePrefab( NodeType nodeType )
    {
        switch (nodeType)
        {
            case NodeType.Battle:
                return battleNode;
            
            case NodeType.Elite:
                return EliteNode;

            case NodeType.Event:
                return EventNode;

            case NodeType.Shop:
                return ShopNode;

            case NodeType.Rest:
                return RestNode;

            case NodeType.Boss:
                return BossNode;

            default:
                return null;
        }
    }


}
