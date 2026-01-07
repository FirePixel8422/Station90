using System.Collections.Generic;
using Unity.Mathematics;


public struct AStarPathfinder
{
    private const int NORMAL_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;
    private static readonly MinMaxFloat RANDOM_COST_MULTIPLIER = new MinMaxFloat(0.5f, 1.5f);

    public GridFloor GridFloor;
    public float3 CurrentPos;
    public float3 TargetPos;
    public MinMaxFloat FuzzynessMinMax;
    public List<float3> Path;

    public bool Schedule()
    {
        Node startNode = GridFloor.NodeFromWorldPoint(CurrentPos);
        Node targetNode = GridFloor.NodeFromWorldPoint(TargetPos);

        GridFloor.PreparePathFindData();
        NodeHeap openNodes = GridFloor.OpenNodes;
        HashSet<Node> closedNodes = GridFloor.ClosedNodes;
        Node[] neighbours = GridFloor.Neighbours;

        Node cNeighbour;

        int currentNodeGridId;
        int cNeighbourDist;
        int newMovementCostToNeigbour;

        int2 cGridPos;
        int2 targetGridPos;

        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                return TryRetracePath(startNode, targetNode, Path);
            }

            GridFloor.GetNeigbours(currentNode, neighbours, out int neighbourCount);

            for (int i = 0; i < neighbourCount; i++)
            {
                cNeighbour = neighbours[i];

                if (cNeighbour.IsWalkable == false || closedNodes.Contains(cNeighbour))
                {
                    continue;
                }

                currentNodeGridId = currentNode.GridId;

                cGridPos = GridFloor.GridIdToGridPos(cNeighbour.GridId);
                targetGridPos = GridFloor.GridIdToGridPos(currentNodeGridId);

                cNeighbourDist = GetDistance(cGridPos, targetGridPos, FuzzynessMinMax);
                newMovementCostToNeigbour = currentNode.GCost + cNeighbourDist;

                if (newMovementCostToNeigbour < cNeighbour.GCost || openNodes.Contains(cNeighbour) == false)
                {
                    cNeighbour.GCost = newMovementCostToNeigbour;

                    targetGridPos = GridFloor.GridIdToGridPos(targetNode.GridId);

                    cNeighbour.HCost = GetDistance(cGridPos, targetGridPos, FuzzynessMinMax);
                    cNeighbour.ParentGridId = currentNodeGridId;

                    if (openNodes.Contains(cNeighbour) == false)
                    {
                        openNodes.Add(cNeighbour);
                    }
                }
            }
        }

        return false;
    }

    private bool TryRetracePath(Node startNode, Node endNode, List<float3> path)
    {
        if (endNode == startNode)
        {
            DebugLogger.LogWarning("Target Already Reached");
            return false;
        }

        Node currentNode = endNode;
        path.Clear();

        while (currentNode != startNode)
        {
            path.Add(currentNode.WorldPos);
            currentNode = GridFloor.NodeFromGridId(currentNode.ParentGridId);
        }

        // Reverse when done
        path.Reverse();
        return true;
    }

    private int GetDistance(int2 gridPosA, int2 gridPosB, MinMaxFloat fuzzynessMinMax)
    {
        int distX = math.abs(gridPosA.x - gridPosB.x);
        int distZ = math.abs(gridPosA.y - gridPosB.y);

        float fuzzynessFactor = EzRandom.Range(fuzzynessMinMax);

        if (distX > distZ)
        {
            return (int)((DIAGONAL_MOVE_COST * distZ + NORMAL_MOVE_COST * (distX - distZ)) * fuzzynessFactor);
        }
        else
        {
            return (int)((DIAGONAL_MOVE_COST * distX + NORMAL_MOVE_COST * (distZ - distX)) * fuzzynessFactor);
        }
    }
}
