using System.Collections.Generic;
using UnityEngine;


public class Stalker : UpdateMonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 prevTargetPos;

    [SerializeField] private float updateDelay;
    [SerializeField] private float minForcedUpdateDelay;
    [SerializeField] private float minTargetMovementForUpdate;
    [SerializeField] private float cMoveSpeed;

    [SerializeField] private List<Vector3> path;

    [SerializeField] private bool drawPathGizmos = true;
    [SerializeField] private Color pathNodesColor = Color.black;

    [SerializeField] private bool hasActivePath;


    public float elapsedTimeSinceLastUpdate;

    protected override void OnUpdate()
    {
        float deltaTime = Time.deltaTime;
        bool pathUpdateQueued = false;

        elapsedTimeSinceLastUpdate += deltaTime;

        bool targetMovedEnough = Vector3.Distance(target.position, prevTargetPos) > minTargetMovementForUpdate;

        if (targetMovedEnough || elapsedTimeSinceLastUpdate > updateDelay || elapsedTimeSinceLastUpdate > minForcedUpdateDelay)
        {
            prevTargetPos = target.position;
            elapsedTimeSinceLastUpdate = 0;

            pathUpdateQueued = true;
        }

        if (hasActivePath)
        {
            MoveStalker(deltaTime, pathUpdateQueued);
        }
        else if (pathUpdateQueued)
        {
            hasActivePath = RecalculateNextPath();
        }
    }

    public bool RecalculateNextPath()
    {
        // Position to move to
        Vector3 destinationPos = target.position;

        return AStarPathfinder.TryGetPathToTarget(transform.position, destinationPos, path);
    }

    private void MoveStalker(float deltaTime, bool pathUpdateQueued)
    {
        float initialMovement = cMoveSpeed * deltaTime;
        float movementLeft = initialMovement;

        bool tileReached;


        while (movementLeft > 0)
        {
            (tileReached, movementLeft) = MoveTowardsNode(movementLeft);

            if (tileReached)
            {
                //call new path calculation if update is queued, end the movement loop
                if (pathUpdateQueued)
                {
                    RecalculateNextPath();
                    break;
                }

                //next path node
                path.RemoveAt(0);

                //if current path is completed, end the movement loop
                if (path.Count == 0)
                {
                    hasActivePath = false;
                    break;
                }
            }
        }
    }
    private (bool, float) MoveTowardsNode(float maxDistanceThisFrame)
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = path[0];


        // Calculate the direction vector and its magnitude
        Vector3 vectorToTarget = targetPosition - currentPosition;

        float distanceToTarget = vectorToTarget.magnitude;

        // Check if we can reach the target in this frame
        if (distanceToTarget <= maxDistanceThisFrame)
        {
            // Move directly to the target and calculate remaining distance
            transform.position = targetPosition;

            // Calculate the remainder of speed left
            float remainder = maxDistanceThisFrame - distanceToTarget;

            return (true, remainder);
        }
        else
        {
            // Move partially towards the target with a direction to the next node
            transform.position = currentPosition + vectorToTarget.normalized * maxDistanceThisFrame;

            // No remainder since we couldn't reach the next node
            return (false, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawPathGizmos == true && path != null)
        {
            Gizmos.color = pathNodesColor;
            for (int i2 = 0; i2 < path.Count; i2++)
            {
                Gizmos.DrawCube(path[i2], 0.9f * GridManager.Instance.nodeSize * Vector3.one);
            }
        }
    }
}