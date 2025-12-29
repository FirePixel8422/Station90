using UnityEngine;


public class PlayerCamAnimator : UpdateMonoBehaviour
{
    [SerializeField] private Transform cameraAnimator;
    [SerializeField] private Transform cam;

    private bool IsAnimating;
    private TransformLerpHelper animData;



    protected override void OnUpdate()
    {
        if (IsAnimating == false) return;

        animData.ApplyPositionAndRotation(Time.time, ref cam, out bool lerpCompleted);
        if (lerpCompleted) IsAnimating = false;
    }

    public void AnimateCameraToPoint(Vector3 targetPoint, Quaternion targetRot, float lerpTime)
    {
        IsAnimating = true;
        PlayerDataLibrary.Controller.IsControlEnabled = false;
        PlayerDataLibrary.FlashLight.IsForceDisabled = true;

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
        PlayerDataLibrary.Controller.IsControlEnabled = true;
        PlayerDataLibrary.FlashLight.IsForceDisabled = false;
    }
}