using UnityEngine;
using System.Collections.Generic;
using MapSystem;

public class MapNode
{
    public NodeType Type;
    public Vector2 Position;
    public List<MapNode> ConnectedNodes = new List<MapNode>();
    public string UniqueId;
}
