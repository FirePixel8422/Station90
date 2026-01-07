using System;
using Unity.Mathematics;


public class Node : IComparable<Node>
{
    public int LayerId;

    public bool IsWalkable;
    public float3 WorldPos;

    public int GridId;
    public int ParentGridId;

    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public int HeapIndex;

    public Node(bool walkable, float3 worldPos, int gridId, int layerId)
    {
        IsWalkable = walkable;
        WorldPos = worldPos;
        GridId = gridId;
        LayerId = layerId;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return -compare;
    }
        
    public override bool Equals(object obj)
    {
        return obj is Node other && Equals(other);
    }
    public bool Equals(Node other)
    {
        return other != null && GridId == other.GridId;
    }
    public override int GetHashCode()
    {
        return GridId.GetHashCode();
    }
}