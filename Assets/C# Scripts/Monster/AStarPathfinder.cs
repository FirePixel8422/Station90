using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



public static class AStarPathfinder
{
    public static bool TryGetPathToTarget(Vector3 startPos, Vector3 targetPos, List<Vector3> path)
    {
        Node startNode = GridManager.Instance.NodeFromWorldPoint(startPos);
        Node targetNode = GridManager.Instance.NodeFromWorldPoint(targetPos);

        Heap<Node> openNodes = new Heap<Node>(GridManager.Instance.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();


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


            foreach (Node neigbour in GridManager.Instance.GetNeigbours(currentNode))
            {
                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }

                int2 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist;

                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;

                    neigbour.hCost = GetDistance(neigbour.gridPos, targetNode.gridPos);
                    neigbour.parentGridPos = currentNodeGridPos;

                    if (!openNodes.Contains(neigbour))
                    {
                        openNodes.Add(neigbour);
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
            Debug.LogWarning("Target Already Reached");
            return false;
        }

        Node currentNode = endNode;

        while (currentNode != startNode)
        {

            if (currentNode != endNode)
            {
                path.Clear();
                return false;
            }


            path.Add(currentNode.worldPos);

            currentNode = GridManager.NodeFromGridId(currentNode.parentGridPos);
        }

        // Reverse when done
        path.Reverse();

        return true;
    }


    private const int NORMAL_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;


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
