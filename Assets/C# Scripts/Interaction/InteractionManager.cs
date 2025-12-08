using System.Collections.Generic;
using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public static List<Interactable> Interactables { get; private set; } = new List<Interactable>();

    private InteractionController player;
    private Interactable activeInteractable;



    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void Start()
    {
        player = InteractionController.Instance;
    }


    private void OnUpdate()
    {
        int itemCount = Interactables.Count;
        if (itemCount == 0) return;

        Vector3 playerPos = player.InteractionTransformPos;
        Interactable newItem;

        float closestValidDist = float.MaxValue;
        int closestValidItemId = -1;

        for (int i = 0; i < itemCount; i++)
        {
            newItem = Interactables[i];

            // Check if newItem is closer to player then possible previously selected valid item
            float dist = Vector3.Distance(playerPos, newItem.transform.position);
            if (dist > closestValidDist) continue;

            bool isItemInRange = newItem.IsInteractable && dist <= player.InteractionRange;
            bool isItemInFront = Vector3.Dot(Camera.main.transform.forward, (newItem.InteractionPosition - playerPos).normalized) > player.InteractionDot;

            if (isItemInRange && isItemInFront)
            {
                closestValidDist = dist;
                closestValidItemId = i;
            }
        }

        UpdateItems(closestValidItemId);
    }

    /// <summary>
    /// Update new item and previously selected item
    /// </summary>
    private void UpdateItems(int closestValidItemId)
    {
        if (closestValidItemId != -1)
        {
            if (Interactables[closestValidItemId] == activeInteractable) return;

            Interactables[closestValidItemId].SetPopupActiveState(true);

            if (activeInteractable != null)
            {
                activeInteractable.SetPopupActiveState(false);
            }
            activeInteractable = Interactables[closestValidItemId];
        }
        else
        {
            if (activeInteractable != null)
            {
                activeInteractable.SetPopupActiveState(false);
                activeInteractable = null;
            }
        }
    }
}