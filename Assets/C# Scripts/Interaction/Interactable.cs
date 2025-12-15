using TMPro;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    [SerializeField] private Vector3 interactionOffset;
    public Vector3 InteractionPosition => transform.position + interactionOffset;

    [SerializeField] private float interactionCooldown;
    [SerializeField] private bool isInteractable = true;
    public bool IsInteractable => isInteractable && Time.time >= lastInteractGlobalTime + interactionCooldown;


    private float lastInteractGlobalTime;



    private void Awake()
    {
        lastInteractGlobalTime = -interactionCooldown;
    }

    protected virtual void OnEnable() => InteractionManager.Interactables.Add(this);
    protected virtual void OnDisable() => InteractionManager.Interactables.Remove(this);

    public bool TryInteract()
    {
        if (IsInteractable)
        {
            lastInteractGlobalTime = Time.time;
            OnInteract();
            return true;
        }
        return false;
    }
    public void Release()
    {
        OnRelease();
    }
    protected virtual void OnInteract() { }
    protected virtual void OnRelease() { }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(InteractionPosition, 0.25f);
    }
}