using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



[System.Serializable]
public class GridFloor
{
    public float3 gridSize;
    public float3 gridPosition;
    [Range(0.1f, 5)]
    public float nodeSize;

    public float3 GridSize => gridSize;
    public float3 GridPosition => gridPosition;
    public float NodeSize => nodeSize;

    public Node[] Grid { get; private set; }
    
    public int GridSizeX { get; private set; }
    public int GridSizeZ { get; private set; }
    public int GridLength => GridSizeX * GridSizeZ;


    public NodeHeap OpenNodes;
    public HashSet<Node> ClosedNodes;
    public Node[] Neighbours { get; private set; }


    public void Create(LayerMask walkableMask, Node[] neighbours)
    {
        float3 gridSize = GridSize;
        float3 gridPosition = GridPosition;
        float nodeSize = NodeSize;

        GridSizeX = Mathf.RoundToInt(GridSize.x / nodeSize);
        GridSizeZ = Mathf.RoundToInt(GridSize.z / nodeSize);

        Grid = new Node[GridLength];
        float3 worldBottomLeft =
            gridPosition -
            new float3(gridSize.x * 0.5f, 0, 0) -
            new float3(0, 0, gridSize.z * 0.5f);

        for (int gridId = 0; gridId < GridLength; gridId++)
        {
            int2 gridPos = new int2(
                gridId % GridSizeX,
                gridId / GridSizeX);

            Vector3 worldPoint =
                worldBottomLeft +
                new float3(1, 0, 0) * (gridPos.x * nodeSize + nodeSize * 0.5f) +
                new float3(0, 0, 1) * (gridPos.y * nodeSize + nodeSize * 0.5f);

            bool isWalkable = false;
            int layer = 0;
            float overrideMaxStep = -1;

            if (Physics.Raycast(worldPoint + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 15f))
            {
                int hitLayer = hit.transform.gameObject.layer;
                bool inMask = (walkableMask & (1 << hitLayer)) != 0;

                isWalkable = inMask && hit.distance < 1;
                layer = hitLayer;

                if (hit.transform.TryGetComponent(out AStarTerrainStair terrainObject))
                {
                    overrideMaxStep = terrainObject.OverrideMaxStep;
                    isWalkable = true;
                }
            }
            Grid[gridId] = new Node(isWalkable, hit.point, gridId, layer, overrideMaxStep);
        }

        OpenNodes = new NodeHeap(GridLength);
        ClosedNodes = new HashSet<Node>(GridLength / 2);
        Neighbours = neighbours;
    }

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