using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class Stalker : UpdateMonoBehaviour
{
    [SerializeField] private IPathfindTarget target;
    [SerializeField] private Vector3 prevTargetPos;

    [SerializeField] private float updateDelay;
    [SerializeField] private float minForcedUpdateDelay;
    [SerializeField] private float minTargetMovementForUpdate;
    [SerializeField] private float cMoveSpeed;
    [SerializeField] private float maxStairYStep;

    [SerializeField] private List<float3> path;

    [SerializeField] private bool drawPathGizmos = true;
    [SerializeField] private Color pathNodesColor = Color.black;
    public int GridFloorId => transform.position.y > 2 ? 1 : 0;

    private float elapsedTimeSinceLastUpdate;


    private void Start()
    {
        target = PlayerDataLibrary.PathfindTarget;
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.deltaTime;

        elapsedTimeSinceLastUpdate += deltaTime;

        bool targetMovedEnough = Vector3.Distance(target.Position, prevTargetPos) > minTargetMovementForUpdate;

        if (targetMovedEnough || elapsedTimeSinceLastUpdate > minForcedUpdateDelay)
        {
            prevTargetPos = target.Position;
            elapsedTimeSinceLastUpdate = 0;
            RecalculateNextPath();
        }
        MoveStalker(deltaTime);
    }

    public bool RecalculateNextPath()
    {
        // Position to move to
        Vector3 destinationPos = target.Position;

        AStarPathfinder pathfinder = new AStarPathfinder()
        {
            CurrentPos = transform.position,
            TargetPos = destinationPos,
            FuzzynessMinMax = new MinMaxFloat(1, 1),
            MaxYStep = 1,
            GridFloor = GridManager.Instance.gridFloors[target.GridFloorId],
            Path = path
        };

        // System.Diagnostics.Stopwatch sw =  System.Diagnostics.Stopwatch.StartNew();
        bool succes = pathfinder.Schedule();
        //DebugLogger.Log(sw.ElapsedMilliseconds + "ms");
        return succes;
    }

    private void MoveStalker(float deltaTime)
    {
        float initialMovement = cMoveSpeed * deltaTime;
        float movementLeft = initialMovement;

        while (path.Count > 0 && movementLeft > 0)
        {
            if (MoveTowardsNode(ref movementLeft))
            {
                // Next path node
                path.RemoveAt(0);
            }
        }
    }
    private bool MoveTowardsNode(ref float maxDistanceThisFrame)
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
            maxDistanceThisFrame -= distanceToTarget;

            return true;
        }
        else
        {
            // Move partially towards the target with a direction to the next node
            transform.position = currentPosition + vectorToTarget.normalized * maxDistanceThisFrame;

            // No remainder since we couldn't reach the next node
            maxDistanceThisFrame = 0;
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (drawPathGizmos == true && path != null)
        {
            Gizmos.color = pathNodesColor;
            for (int i2 = 0; i2 < path.Count; i2++)
            {
                Gizmos.DrawCube(path[i2], 0.9f * GridManager.Instance.gridFloors[target.GridFloorId].NodeSize * Vector3.one);
            }
        }
    }
}