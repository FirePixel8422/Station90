using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractionController : UpdateMonoBehaviour
{   

    [SerializeField] private Transform interactionTransform;
    public Vector3 InteractionTransformPos => interactionTransform.position;


    [Header("Crosshair:")]
    [SerializeField] private CrosshairHandler crosshairHandler;
    [Header("Inventory:")]
    [SerializeField] private PlayerInventory inventory;


    [SerializeField] private float interactionRange;
    [SerializeField] private float interactionDot;
    public float InteractionRange => interactionRange;
    public float InteractionDot => interactionDot;


    private Interactable heldInteractable;
    private Interactable lastHeldInteractable;


    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (InteractionManager.TryGetActiveItem(out heldInteractable) && heldInteractable.IsInteractable)
            {
                lastHeldInteractable = heldInteractable;
                heldInteractable.TryInteract();
            }
        }
        if (ctx.canceled)
        {
            if (heldInteractable != null)
            {
                heldInteractable.Release();
                heldInteractable = null;
            }
        }
    }

    private void Awake()
    {
        crosshairHandler.Init();
    }
    protected override void OnUpdate()
    {
        crosshairHandler.OnUpdate(InteractionManager.IsAnyItemSelected);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, InteractionRange);
    }
}
