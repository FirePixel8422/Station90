using System.Collections;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    [SerializeField] private float interactionCooldown;
    [SerializeField] private bool interactable = true;
    public bool IsInteractable => interactable && Time.time >= lastInteractGlobalTime + interactionCooldown;

    [SerializeField] 

    private float lastInteractGlobalTime;

    private bool popupActive;
    private Coroutine activePopupFadeCO;


    private void OnEnable() => InteractionManager.Interactables.Add(this);
    private void OnDisable() => InteractionManager.Interactables.Remove(this);


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


    public void SetPopupActiveState(bool state)
    {
        // If the value is changing
        if (popupActive != state)
        {
            // If the popup becomes enabled
            if (state == true)
            {
                if (activePopupFadeCO != null)
                {
                    StopCoroutine(activePopupFadeCO);
                }
                activePopupFadeCO = StartCoroutine(FadeInPopup());
            }
            // If the popup becomes disabled
            else
            {
                if (activePopupFadeCO != null)
                {
                    StopCoroutine(activePopupFadeCO);
                }
                activePopupFadeCO = StartCoroutine(FadeOutPopup());
            }
        }
        popupActive = state;
    }

    private IEnumerator FadeInPopup()
    {
        while (true)
        {
            yield return null;
        }
    }
    private IEnumerator FadeOutPopup()
    {
        while (true)
        {
            yield return null;
        }
    }
}