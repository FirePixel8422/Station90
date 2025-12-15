using UnityEngine;



[System.Serializable]
public struct TransformLerpHelper
{
    public Vector3 StartPoint;
    public Quaternion StartRot;

    public Vector3 TargetPoint;
    public Quaternion TargetRot;

    public float AnimStartTime;
    public float AnimDuration;

    public TransformLerpHelper(Vector3 startPoint, Vector3 targetPoint, Quaternion startRot, Quaternion targetRot, float animStartTime, float animDuration)
    {
        StartPoint = startPoint;
        StartRot = startRot;
        TargetPoint = targetPoint;
        TargetRot = targetRot;
        AnimStartTime = animStartTime;
        AnimDuration = animDuration;
    }

    /// <summary>
    /// Apply Lerped pos and rot to targetTransform
    /// </summary>
    public void ApplyPositionAndRotation(float globalTime, ref Transform targetTransform, out bool lerpCompleted)
    {
        float t = (globalTime - AnimStartTime) * AnimDuration;

        Vector3 pos = Vector3.Lerp(StartPoint, TargetPoint, t);
        Quaternion rot = Quaternion.Lerp(StartRot, TargetRot, t);

        targetTransform.SetPositionAndRotation(pos, rot);

        lerpCompleted = t >= 1;
    }
}