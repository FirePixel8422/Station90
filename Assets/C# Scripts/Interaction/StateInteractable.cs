using UnityEngine;


public class StateInteractable : Interactable
{
    protected bool isInteractableActive;

    protected override void OnInteract()
    {
        isInteractableActive = !isInteractableActive;

        if (isInteractableActive)
        {
            OnToggleActivate();
        }
        else
        {
            OnToggleDeActivate();
        }
    }

    protected virtual void OnToggleActivate() { }
    protected virtual void OnToggleDeActivate() { }
}