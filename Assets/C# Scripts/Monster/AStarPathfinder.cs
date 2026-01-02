using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



public static class AStarPathfinder
{
    private const int NORMAL_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;

    public static bool TryGetPathToTarget(Vector3 startPos, Vector3 targetPos, List<Vector3> path)
    {
        Node startNode = GridManager.Instance.NodeFromWorldPoint(startPos);
        Node targetNode = GridManager.Instance.NodeFromWorldPoint(targetPos);

        NodeHeap openNodes = new NodeHeap(GridManager.Instance.GridLength);
        HashSet<Node> closedNodes = new HashSet<Node>(GridManager.Instance.GridLength / 2);
        Node[] neighbours = new Node[8];

        Node cNeighbour;

        int currentNodeGridId;
        int cNeighbourDist;
        int newMovementCostToNeigbour;

        int2 gridPosA;
        int2 gridPosB;

        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                bool pathSucces = RetracePath(startNode, targetNode, path);

                return pathSucces;
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

                gridPosA = GridManager.GridIdToGridPos(currentNodeGridId);
                gridPosB = GridManager.GridIdToGridPos(cNeighbour.GridId);

                cNeighbourDist = GetDistance(gridPosA, gridPosB);
                newMovementCostToNeigbour = currentNode.GCost + cNeighbourDist;

                if (newMovementCostToNeigbour < cNeighbour.GCost || openNodes.Contains(cNeighbour) == false)
                {
                    cNeighbour.GCost = newMovementCostToNeigbour;

                    gridPosA = GridManager.GridIdToGridPos(cNeighbour.GridId);
                    gridPosB = GridManager.GridIdToGridPos(targetNode.GridId);

                    cNeighbour.HCost = GetDistance(gridPosA, gridPosB);
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

    private static bool RetracePath(Node startNode, Node endNode, List<Vector3> path)
    {
        path.Clear();

        if (endNode == startNode)
        {
            DebugLogger.LogWarning("Target Already Reached");
            return false;
        }

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.WorldPos);
            currentNode = GridManager.NodeFromGridId(currentNode.ParentGridId);
        }

        // Reverse when done
        path.Reverse();
        return true;
    }

    private static int GetDistance(int2 gridPosA, int2 gridPosB)
    {
        int distX = math.abs(gridPosA.x - gridPosB.x);
        int distZ = math.abs(gridPosA.y - gridPosB.y);

        if (distX > distZ)
        {
            return DIAGONAL_MOVE_COST * distZ + NORMAL_MOVE_COST * (distX - distZ);
        }
        else
        {
            return DIAGONAL_MOVE_COST * distX + NORMAL_MOVE_COST * (distZ - distX);
        }
    }
}
