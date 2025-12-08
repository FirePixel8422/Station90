using UnityEngine;
using UnityEngine.InputSystem;


public class InteractionController : MonoBehaviour
{
    public static InteractionController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform interactionTransform;
    public Vector3 InteractionTransformPos => interactionTransform.position;


    public float InteractionRange;
    public float InteractionDot;
    private Interactable heldInteractable;


    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, InteractionRange))
            {
                if (hit.collider.TryGetComponent(out heldInteractable) && heldInteractable.IsInteractable)
                {
                    heldInteractable.TryInteract();
                }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, InteractionRange);
    }
}
