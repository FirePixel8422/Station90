using FirePixel.Utility;
using UnityEngine;
using UnityEngine.InputSystem;


public class MonitorInteractable : StateInteractable
{
    [SerializeField] private Transform viewMonitorTransform;

    [SerializeField] private float enterMonitorLerpTime;
    [SerializeField] private float exitMonitorLerpTime;

    private Vector2 mouseDelta;

    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>() * PlayerSettingsHandler.Settings.MouseSensitivity;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateScheduler.RegisterUpdate(OnUpdate);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UpdateScheduler.UnRegisterUpdate(OnUpdate);
    }


    protected override void OnToggleActivate()
    {
        PlayerDataLibrary.CamAnimator.AnimateCameraToPoint(viewMonitorTransform.position, viewMonitorTransform.rotation, enterMonitorLerpTime);
    }
    protected override void OnToggleDeActivate()
    {
        PlayerDataLibrary.CamAnimator.UnlockCameraToOrigin(viewMonitorTransform.rotation, exitMonitorLerpTime);
    }

    private void OnUpdate()
    {
        if (interactableActiveState)
        {

        }
    }
}
