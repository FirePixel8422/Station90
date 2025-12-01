using System.Collections.Generic;
using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public static List<Interactable> Interactables { get; private set; }

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
        Interactable item;

        for (int i = 0; i < itemCount; i++)
        {
            item = Interactables[i];

            bool itemInRange = item.IsInteractable && Vector3.Distance(playerPos, item.transform.position) <= player.InteractionRange;
            item.SetPopupActiveState(itemInRange);
        }
    }
}