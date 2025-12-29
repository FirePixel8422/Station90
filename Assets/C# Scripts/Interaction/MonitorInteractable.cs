using UnityEngine;
using UnityEngine.InputSystem;


public class MonitorInteractable : StateInteractable
{
    [SerializeField] private InputActionReference onMoveActionRef;
    [SerializeField] private Transform[] viewMonitorTransforms;

    [SerializeField] private float enterMonitorLerpTime;
    [SerializeField] private float swapMonitorLerpTime;
    [SerializeField] private float exitMonitorLerpTime;

    private int activeMonitorId;


    private void Awake()
    {
        onMoveActionRef.action.performed += OnMove;
        onMoveActionRef.action.canceled += OnMove;
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 moveDelta = ctx.ReadValue<Vector2>();

        if (isInteractableActive)
        {
            // If "s" is pressed, exit monitor
            if (moveDelta.y < 0)
            {
                OnInteract();
            }
            else if (moveDelta.x != 0)
            {
                int prevMonitorId = activeMonitorId;
                if (moveDelta.x > 0)
                {
                    activeMonitorId = Mathf.Min(activeMonitorId + 1, viewMonitorTransforms.Length - 1);
                }
                else
                {
                    activeMonitorId = Mathf.Max(activeMonitorId - 1, 0);
                }

                if(prevMonitorId == activeMonitorId) return;

                PlayerDataLibrary.CamAnimator.AnimateCameraToPoint(viewMonitorTransforms[activeMonitorId].position, viewMonitorTransforms[activeMonitorId].rotation, swapMonitorLerpTime);
            }
        }
    }

    protected override void OnToggleActivate()
    {
        activeMonitorId = Mathf.FloorToInt(viewMonitorTransforms.Length * 0.5f);
        PlayerDataLibrary.CamAnimator.AnimateCameraToPoint(viewMonitorTransforms[activeMonitorId].position, viewMonitorTransforms[activeMonitorId].rotation, enterMonitorLerpTime);
    }
    protected override void OnToggleDeActivate()
    {
        int centerMonitorId = Mathf.FloorToInt(viewMonitorTransforms.Length * 0.5f);
        PlayerDataLibrary.CamAnimator.UnlockCameraToOrigin(viewMonitorTransforms[centerMonitorId].rotation, exitMonitorLerpTime);
    }

    private void OnDestroy()
    {
        onMoveActionRef.action.performed -= OnMove;
        onMoveActionRef.action.canceled -= OnMove;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < viewMonitorTransforms.Length; i++)
        {
            Gizmos.DrawWireSphere(viewMonitorTransforms[i].position, 0.1f);
            Gizmos.DrawLine(viewMonitorTransforms[i].position, viewMonitorTransforms[i].position + viewMonitorTransforms[i].forward);
        }
    }
}
