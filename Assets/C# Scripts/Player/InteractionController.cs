using UnityEngine;
using UnityEngine.InputSystem;


public class InteractionController : MonoBehaviour
{
    public static InteractionController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }


    public float InteractionRange;
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
                heldInteractable = null;
                heldInteractable.Release();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, InteractionRange);
    }
}
