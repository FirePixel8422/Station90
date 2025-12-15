using UnityEngine;


public class StateInteractable : Interactable
{
    protected bool interactableActiveState;

    protected override void OnInteract()
    {
        interactableActiveState = !interactableActiveState;

        if (interactableActiveState)
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