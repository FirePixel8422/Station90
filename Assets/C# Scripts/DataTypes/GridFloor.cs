using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



[System.Serializable]
public class GridFloor
{
    public float3 GridSize;
    public float3 GridPosition;

    [Range(0.1f, 5)]
    public float NodeSize;

    public Node[] Grid;

    [HideInInspector]
    public int GridSizeX, GridSizeZ;
    public int GridLength => GridSizeX * GridSizeZ;

    public NodeHeap OpenNodes;
    public HashSet<Node> ClosedNodes;
    public Node[] Neighbours => GridManager.Neighbours;


    public void PreparePathFindData()
    {
        OpenNodes.Clear();
        ClosedNodes.Clear();
    }

    public void GetNeigbours(Node node, Node[] neighbours, out int neighbourCount)
    {
        neighbourCount = 0;
        int2 gridPos = new int2(
                node.GridId % GridSizeX,
                node.GridId / GridSizeX);

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Skip Center: 0,0
                if (x == 0 && z == 0) continue;
                // Skip Center and Diagonals
                // if (math.abs(x) == math.abs(z)) continue;

                int checkX = gridPos.x + x;
                int checkZ = gridPos.y + z;

                if (checkX >= 0 && checkX < GridSizeX && checkZ >= 0 && checkZ < GridSizeZ)
                {
                    int checkGridId = checkX + checkZ * GridSizeX;
                    neighbours[neighbourCount++] = Grid[checkGridId];
                }
            }
        }
    }
    public Node NodeFromWorldPoint(float3 worldPosition)
    {
        float3 localPos = worldPosition - GridPosition;

        float percentX = (localPos.x + GridSize.x * 0.5f) / GridSize.x;
        float percentZ = (localPos.z + GridSize.z * 0.5f) / GridSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((GridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((GridSizeZ - 1) * percentZ);

        int gridId = x + z * GridSizeX;

        return Grid[gridId];
    }
    public Node NodeFromGridId(int gridId)
    {
        return Grid[gridId];
    }
    public int2 GridIdToGridPos(int gridId)
    {
        return new int2(
            gridId % GridSizeX,
            gridId / GridSizeX);
    }
}