using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }


    [SerializeField] private LayerMask obstructionMask;

    public GridFloor[] gridFloors;

#pragma warning disable UDR0001
    public static Node[] Neighbours;
#pragma warning restore UDR0001


    private void Awake()
    {
        Instance = this;
        CreateGrids();
    }

    private void CreateGrids()
    {
        for (int i = 0; i < gridFloors.Length; i++)
        {
            GridFloor cFloor = gridFloors[i];

            float3 gridSize = cFloor.GridSize;
            float3 gridPosition = cFloor.GridPosition;
            float nodeSize = cFloor.NodeSize;

            cFloor.GridSizeX = Mathf.RoundToInt(cFloor.GridSize.x / nodeSize);
            cFloor.GridSizeZ = Mathf.RoundToInt(cFloor.GridSize.z / nodeSize);

            cFloor.Grid = new Node[cFloor.GridLength];
            float3 worldBottomLeft = 
                gridPosition -
                new float3(gridSize.x * 0.5f, 0, 0) -
                new float3(0, 0, gridSize.z * 0.5f);

            for (int gridId = 0; gridId < cFloor.GridLength; gridId++)
            {
                int2 gridPos = new int2(
                    gridId % cFloor.GridSizeX,
                    gridId / cFloor.GridSizeX);

                Vector3 worldPoint =
                    worldBottomLeft +
                    new float3(1, 0, 0) * (gridPos.x * nodeSize + nodeSize / 2) +
                    new float3(0, 0, 1) * (gridPos.y * nodeSize + nodeSize / 2);

                bool isWalkable = false;
                int layer = 0;
                if (Physics.Raycast(worldPoint + Vector3.up * 10, Vector3.down, out RaycastHit hit, 100))
                {
                    int hitLayer = hit.transform.gameObject.layer;
                    bool inMask = (obstructionMask & (1 << hitLayer)) != 0;

                    layer = hitLayer;
                    isWalkable = !inMask;
                }
                cFloor.Grid[gridId] = new Node(isWalkable, worldPoint, gridId, layer);
            }

            cFloor.OpenNodes = new NodeHeap(cFloor.GridLength);
            cFloor.ClosedNodes = new HashSet<Node>(cFloor.GridLength / 2);
        }
        Neighbours = new Node[8];
    }


#if UNITY_EDITOR

    [SerializeField] private bool drawNodeColorGizmos = false;
    [SerializeField] private Color[] nodeLayerColors;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < gridFloors.Length; i++)
        {
            GridFloor cFloor = gridFloors[i];
            Node[] cGrid = cFloor.Grid;

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(cFloor.GridPosition, new Vector3(cFloor.GridSize.x, 0.5f, cFloor.GridSize.z));

            if (Application.isPlaying == false || drawNodeColorGizmos == false) return;
            {
                for (int i2 = 0; i2 < cFloor.GridSizeX; i2++)
                {
                    for (int i3 = 0; i3 < cFloor.GridSizeZ; i3++)
                    {
                        int gridId = i2 + i3 * cFloor.GridSizeX;

                        Gizmos.color = nodeLayerColors[cGrid[gridId].IsWalkable ? 0 : 1];
                        Gizmos.DrawCube(cGrid[gridId].WorldPos, Vector3.one * cFloor.NodeSize * 0.9f);
                    }
                }
            }
        }
    }

#endif
}
