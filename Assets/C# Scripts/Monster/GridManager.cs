using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }


    [SerializeField] private LayerMask walkableMask;

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
        Neighbours = new Node[8];
        for (int i = 0; i < gridFloors.Length; i++)
        {
            gridFloors[i].Create(walkableMask, Neighbours);
        }
    }


#if UNITY_EDITOR

    [SerializeField] private bool drawNodeColorGizmos = false;
    [SerializeField] private Color nodeColor;
    [SerializeField] private Color nodeStairColor;
    //[SerializeField] private float[] wastes;

    private void OnDrawGizmos()
    {
        //wastes = new float[gridFloors.Length];

        for (int i = 0; i < gridFloors.Length; i++)
        {
            GridFloor cFloor = gridFloors[i];

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(cFloor.GridPosition, new Vector3(cFloor.GridSize.x, 0.5f, cFloor.GridSize.z));

            if (Application.isPlaying && drawNodeColorGizmos)
            {
                Node[] cGrid = cFloor.Grid;

                float wasteTotal = 0;

                for (int i2 = 0; i2 < cFloor.GridSizeX; i2++)
                {
                    for (int i3 = 0; i3 < cFloor.GridSizeZ; i3++)
                    {
                        int gridId = i2 + i3 * cFloor.GridSizeX;

                        if (cGrid[gridId].IsWalkable == false)
                        {
                            wasteTotal += 1;
                            continue;
                        }

                        Gizmos.color = cGrid[gridId].OverrideMaxStep == -1 ? nodeColor : nodeStairColor;

                        Gizmos.DrawWireCube(cGrid[gridId].WorldPos, 0.85f * cFloor.NodeSize * Vector3.one);
                    }
                }
                //wastes[i] = wasteTotal / (cFloor.GridSizeX * cFloor.GridSizeZ) * 100;
            }
        }

    }

#endif
}
