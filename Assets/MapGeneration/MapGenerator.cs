using UnityEngine;
using System.Collections.Generic;
using MapSystem;

public class MapGenerator : MonoBehaviour
{
    [ExecuteInEditMode]
    [Header("Node Type Limits")]
    public int maxEnemyCount = 20;
    public int maxEliteCount = 5;
    public int maxShopCount = 3;
    public int maxTreasureCount = 6;

    [Header("Map Settings")]
    public int numberOfLevels = 8;
    public int minNodesPerLevel = 2;
    public int maxNodesPerLevel = 4;
    public float horizontalSpacing = 5f;
    public float verticalSpacing = 6f;

    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject elitePrefab;
    public GameObject bossPrefab;
    public GameObject treasurePrefab;
    public GameObject shopPrefab;

    public List<List<MapNode>> mapLevels = new List<List<MapNode>>();
    
    int currentEnemyCount = 0;
    int currentEliteCount = 0;
    int currentShopCount = 0;
    int currentTreasureCount = 0;
    private bool bossAssigned = false;


    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearOldNodes();

        bossAssigned = false;

        mapLevels.Clear();

        currentEnemyCount = 0;
        currentEliteCount = 0;
        currentShopCount = 0;
        currentTreasureCount = 0;

        for (int level = 0; level < numberOfLevels; level++)
        {
            int nodesCount = Random.Range(minNodesPerLevel, maxNodesPerLevel + 1);
            List<MapNode> currentLevelNodes = new List<MapNode>();

            for (int i = 0; i < nodesCount; i++)
            {
                MapNode node = new MapNode();

                node.Type = GetRandomNodeType(level);
                node.Position = new Vector2(i * horizontalSpacing, -level * verticalSpacing);

                currentLevelNodes.Add(node);
            }
            mapLevels.Add(currentLevelNodes);
        }

        ConnectNodes();
        InstantiateMapVisuals();

    }

    void InstantiateMapVisuals()
    {
        foreach (var level in mapLevels)
        {
            foreach (var node in level)
            {
                GameObject prefab = GetPrefabForNodeType(node.Type);
                if (prefab != null)
                {
                    Vector3 pos3D = new Vector3(node.Position.x, 0, node.Position.y);
                    Instantiate(prefab, pos3D, Quaternion.identity, this.transform);
                }
            }
        }
    }

    NodeType GetRandomNodeType(int level)
{

    if (level == numberOfLevels - 1 && !bossAssigned)
    {
        bossAssigned = true;
        return NodeType.Boss;
    }

    float roll = Random.value;

    if (roll < 0.6f && currentEnemyCount < maxEnemyCount)
    {
        currentEnemyCount++;
        return NodeType.Enemy;
    }
    else if (roll < 0.75f && currentTreasureCount < maxTreasureCount)
    {
        currentTreasureCount++;
        return NodeType.Treasure;
    }
    else if (roll < 0.85f && currentEliteCount < maxEliteCount)
    {
        currentEliteCount++;
        return NodeType.Elite;
    }
    else if (roll < 0.9f && currentShopCount < maxShopCount)
    {
        currentShopCount++;
        return NodeType.Shop;
    }

    currentEnemyCount++;
    return NodeType.Enemy;
}

    void ConnectNodes()
    {
        for (int level = 0; level < mapLevels.Count - 1; level++)
        {
            List<MapNode> currentLevel = mapLevels[level];
            List<MapNode> nextLevel = mapLevels[level + 1];

            foreach (var node in currentLevel)
            {
                int connections = Random.Range(1, Mathf.Min(3, nextLevel.Count + 1));
                List<int> connectedIndices = new List<int>();

                for (int i = 0; i < connections; i++)
                {
                    int index;
                    do
                    {
                        index = Random.Range(0, nextLevel.Count);
                    } while (connectedIndices.Contains(index));

                    connectedIndices.Add(index);
                    node.ConnectedNodes.Add(nextLevel[index]);
                }
            }
        }
    }

    GameObject GetPrefabForNodeType(NodeType type)
    {
        switch (type)
        {
            case NodeType.Enemy: return enemyPrefab;
            case NodeType.Elite: return elitePrefab;
            case NodeType.Boss: return bossPrefab;
            case NodeType.Treasure: return treasurePrefab;
            case NodeType.Shop: return shopPrefab;
            default: return null;
        }
    }
    
    void ClearOldNodes()
    {
        foreach (Transform child in transform)
        {
        DestroyImmediate(child.gameObject);
        }
    }

}
