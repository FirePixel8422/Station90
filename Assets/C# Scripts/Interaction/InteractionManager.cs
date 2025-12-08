using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public static List<Interactable> Interactables { get; private set; } = new List<Interactable>();

    private InteractionController player;



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

        Vector3 playerPos = player.transform.position;
        Interactable newItem;

        for (int i = 0; i < itemCount; i++)
        {
            newItem = Interactables[i];

            bool isItemInRange = newItem.IsInteractable && Vector3.Distance(playerPos, newItem.transform.position) <= player.InteractionRange;
            bool isItemInFront = Vector3.Dot(player.transform.forward, (newItem.transform.position - playerPos).normalized) > player.InteractionDot;

            if (isItemInRange && isItemInFront)
            {
                newItem.SetPopupActiveState(true);
            }
        }
    }
}