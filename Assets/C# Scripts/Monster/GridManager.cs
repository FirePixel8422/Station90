using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int2 gridSize;
    private NativeGrid2DBatch<Node> grid;



    private void Awake()
    {
        grid = new NativeGrid2DBatch<Node>(gridSize.x, gridSize.y, Allocator.Persistent);
    }

    public void CycleToNextBatch()
    {
        grid.CycleToNextBatch();
    }
}