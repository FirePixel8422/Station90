using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotAngle;

    private float startRot;
    private float randomTimeOffset;


    private void Awake()
    {
        startRot = rotatePoint.eulerAngles.y;
        randomTimeOffset = Random.Range(-25f, 25f);
    }

    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void OnUpdate()
    {
        rotatePoint.rotation = Quaternion.Euler(0, startRot + rotAngle * 0.5f * Mathf.Sin((Time.time + randomTimeOffset) * rotateSpeed), 0);
    }

    public void SetupCamera(Camera monitorCamera)
    {
        monitorCamera.transform.SetParent(cameraHolder, false, false);
    }


#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] private float cameraLineLength;
    [SerializeField] private Vector3 camerLineStartOffset;

    private void OnDrawGizmos()
    {
        if (rotatePoint == null || cameraHolder == null) return;

        Gizmos.color = Color.red;

        Vector3 cameraForward = rotatePoint.forward * camerLineStartOffset.z;
        Vector3 cameraFOV = rotatePoint.right * camerLineStartOffset.x;

        Vector3 directionAngleLeft = Quaternion.AngleAxis(rotAngle * 0.5f, rotatePoint.up) * rotatePoint.forward;
        Vector3 directionAngleRight = Quaternion.AngleAxis(-rotAngle * 0.5f, rotatePoint.up) * rotatePoint.forward;

        Vector3 endPositionLeft = rotatePoint.position + cameraForward - cameraFOV + directionAngleLeft * cameraLineLength;
        Vector3 endPositionRight = rotatePoint.position + cameraForward + cameraFOV + directionAngleRight * cameraLineLength;


        Gizmos.DrawLine(rotatePoint.position + cameraForward - cameraFOV, endPositionLeft);
        Gizmos.DrawLine(rotatePoint.position + cameraForward + cameraFOV, endPositionRight);
    }
#endif
}
