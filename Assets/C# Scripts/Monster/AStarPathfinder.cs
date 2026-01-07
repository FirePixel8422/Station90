using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;



public static class AStarPathfinder
{
    private const int NORMAL_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;
    private static readonly MinMaxFloat RANDOM_COST_MULTIPLIER = new MinMaxFloat(0.5f, 1.5f);

    public static bool TryGetPathToTarget(Vector3 startPos, Vector3 targetPos, List<Vector3> path, MinMaxFloat fuzzynessMinMax)
    {
        Node startNode = GridManager.Instance.NodeFromWorldPoint(startPos);
        Node targetNode = GridManager.Instance.NodeFromWorldPoint(targetPos);

        GridManager.PreparePathFindData();
        NodeHeap openNodes = GridManager.OpenNodes;
        HashSet<Node> closedNodes = GridManager.ClosedNodes;
        Node[] neighbours = GridManager.Neighbours;

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
                return TryRetracePath(startNode, targetNode, path);
            }

            GridManager.Instance.GetNeigbours(currentNode, neighbours, out int neighbourCount);

            for (int i = 0; i < neighbourCount; i++)
            {
                cNeighbour = neighbours[i];

                if (cNeighbour.IsWalkable == false || closedNodes.Contains(cNeighbour))
                {
                    continue;
                }

                currentNodeGridId = currentNode.GridId;

                cGridPos = GridManager.GridIdToGridPos(cNeighbour.GridId);
                targetGridPos = GridManager.GridIdToGridPos(currentNodeGridId);

                cNeighbourDist = GetDistance(cGridPos, targetGridPos, fuzzynessMinMax);
                newMovementCostToNeigbour = currentNode.GCost + cNeighbourDist;

                if (newMovementCostToNeigbour < cNeighbour.GCost || openNodes.Contains(cNeighbour) == false)
                {
                    cNeighbour.GCost = newMovementCostToNeigbour;

                    targetGridPos = GridManager.GridIdToGridPos(targetNode.GridId);

                    cNeighbour.HCost = GetDistance(cGridPos, targetGridPos, fuzzynessMinMax);
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

    private static bool TryRetracePath(Node startNode, Node endNode, List<Vector3> path)
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
            currentNode = GridManager.NodeFromGridId(currentNode.ParentGridId);
        }

        // Reverse when done
        path.Reverse();
        return true;
    }

    private static int GetDistance(int2 gridPosA, int2 gridPosB, MinMaxFloat fuzzynessMinMax)
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
