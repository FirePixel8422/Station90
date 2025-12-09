using UnityEngine;
using UnityEngine.InputSystem;


public class InteractionController : UpdateMonoBehaviour
{
    public static InteractionController Instance { get; private set; }
    

    [SerializeField] private Transform interactionTransform;
    public Vector3 InteractionTransformPos => interactionTransform.position;


    [SerializeField] private CrosshairHandler crosshairHandler;
    [SerializeField] private PlayerInventory inventory;


    [SerializeField] private float interactionRange;
    [SerializeField] private float interactionDot;
    public float InteractionRange => interactionRange;
    public float InteractionDot => interactionDot;


    private Interactable heldInteractable;


    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (InteractionManager.TryGetActiveItem(out heldInteractable) && heldInteractable.IsInteractable)
            {
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
        Instance = this;
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
