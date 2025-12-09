using System.Collections.Generic;
using UnityEngine;


public class InteractionManager : UpdateMonoBehaviour
{
    public static List<Interactable> Interactables { get; private set; } = new List<Interactable>();

    /// <summary>
    /// The interactable item the play is currently looking at
    /// </summary>
    private static Interactable SelectedItem { get; set; }
    public static bool IsAnyItemSelected => SelectedItem != null;


    private InteractionController player;


    private void Start()
    {
        player = InteractionController.Instance;
    }

    protected override void OnUpdate()
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

        SelectedItem = closestValidItemId == -1 ?
            null :
            Interactables[closestValidItemId];
    }

    public static bool TryGetActiveItem(out Interactable item)
    {
        item = SelectedItem;

        return item != null;
    }
}