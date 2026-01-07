using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }


    public static Node[] Grid { get; private set; }

    [SerializeField] private LayerMask obstructionMask;

    [SerializeField] private Vector3 gridSize;
    [SerializeField] private Vector3 gridPosition;

    [Range(0.1f, 5)]
    public float nodeSize;

    [HideInInspector]
    public int gridSizeX, gridSizeZ;
    public int GridLength => gridSizeX * gridSizeZ;

#pragma warning disable UDR0001
    public static NodeHeap OpenNodes;
    public static HashSet<Node> ClosedNodes;
    public static Node[] Neighbours;
#pragma warning restore UDR0001


    private void Awake()
    {
        Instance = this;
        CreateGrid();
    }

    public void CreateGrid()
    {
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeSize);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeSize);

        Grid = new Node[GridLength];
        Vector3 worldBottomLeft = gridPosition - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2;

        for (int gridId = 0; gridId < GridLength; gridId++)
        {
            int2 gridPos = new int2(
                gridId % gridSizeX,
                gridId / gridSizeX);

            Vector3 worldPoint =
                worldBottomLeft +
                Vector3.right * (gridPos.x * nodeSize + nodeSize / 2) +
                Vector3.forward * (gridPos.y * nodeSize + nodeSize / 2);

            bool isWalkable = false;
            int layer = 0;
            if(Physics.Raycast(worldPoint + Vector3.up * 10, Vector3.down, out RaycastHit hit, 100))
            {
                int hitLayer = hit.transform.gameObject.layer;
                bool inMask = (obstructionMask & (1 << hitLayer)) != 0;

                layer = hitLayer;
                isWalkable = !inMask;
            }
            Grid[gridId] = new Node(isWalkable, worldPoint, gridId, layer);
        }

        OpenNodes = new NodeHeap(GridLength);
        ClosedNodes = new HashSet<Node>(GridLength / 2);
        Neighbours = new Node[8];
    }

    public static void PreparePathFindData()
    {
        OpenNodes.Clear();
        ClosedNodes.Clear();
    }

    public void GetNeigbours(Node node, Node[] neighbours, out int neighbourCount)
    {
        neighbourCount = 0;
        int2 gridPos = new int2(
                node.GridId % gridSizeX,
                node.GridId / gridSizeX);

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

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    int checkGridId = checkX + checkZ * gridSizeX;
                    neighbours[neighbourCount++] = Grid[checkGridId];
                }
            }
        }
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - gridPosition;

        float percentX = (localPos.x + gridSize.x * 0.5f) / gridSize.x;
        float percentZ = (localPos.z + gridSize.z * 0.5f) / gridSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        int gridId = x + z * gridSizeX;

        return Grid[gridId];
    }
    public static Node NodeFromGridId(int gridId)
    {
        return Grid[gridId];
    }
    public static int2 GridIdToGridPos(int gridId)
    {
        return new int2(
            gridId % Instance.gridSizeX,
            gridId / Instance.gridSizeX);
    }


#if UNITY_EDITOR

    [SerializeField] private bool drawNodeColorGizmos = false;

    [SerializeField] private Color[] nodeLayerColors;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(gridPosition, new Vector3(gridSize.x, 0.5f, gridSize.z));

        if (Application.isPlaying == false || drawNodeColorGizmos == false) return;
        {
            for (int i2 = 0; i2 < gridSizeX; i2++)
            {
                for (int i3 = 0; i3 < gridSizeZ; i3++)
                {
                    int gridId = i2 + i3 * gridSizeX;

                    Gizmos.color = nodeLayerColors[Grid[gridId].IsWalkable ? 0 : 1];
                    Gizmos.DrawCube(Grid[gridId].WorldPos, Vector3.one * nodeSize * 0.9f);
                }
            }
        }
    }

#endif
}
