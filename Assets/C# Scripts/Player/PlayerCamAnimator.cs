using UnityEngine;


public class PlayerCamAnimator : UpdateMonoBehaviour
{
    [SerializeField] private Transform cameraAnimator;
    [SerializeField] private Transform cam;

    private PlayerController playerController;

    private bool IsAnimating;
    private TransformLerpHelper animData;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    protected override void OnUpdate()
    {
        if (IsAnimating == false) return;

        animData.ApplyPositionAndRotation(Time.time, ref cam, out bool lerpCompleted);
        if (lerpCompleted) IsAnimating = false;
    }

    public void AnimateCameraToPoint(Vector3 targetPoint, Quaternion targetRot, float lerpTime)
    {
        IsAnimating = true;
        playerController.IsControlEnabled = false;

        animData = new TransformLerpHelper(cam.position, targetPoint, cam.rotation, targetRot, Time.time, lerpTime);
    }

    public void UnlockCameraToOrigin(float lerpTime, float regainControlDelay = 0)
    {
        IsAnimating = true;
        animData = new TransformLerpHelper(cam.position, cameraAnimator.position, cam.rotation, cameraAnimator.rotation, Time.time, lerpTime);

        Invoke(nameof(UnlockPlayerControls), lerpTime + regainControlDelay);
    }
    public void UnlockCameraToOrigin(Quaternion targetRot, float lerpTime, float regainControlDelay = 0)
    {
        IsAnimating = true;
        animData = new TransformLerpHelper(cam.position, cameraAnimator.position, cam.rotation, targetRot, Time.time, lerpTime);

        Invoke(nameof(UnlockPlayerControls), lerpTime + regainControlDelay);
    }

    public void UnlockPlayerControls()
    {
        playerController.IsControlEnabled = true;
    }
}