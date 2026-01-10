using Unity.Mathematics;
using UnityEngine;



public class PlayerDataLibrary : MonoBehaviour, IPathfindTarget
{
    public static IPathfindTarget PathfindTarget { get; private set; }
    public static PlayerController Controller { get; private set; }
    public static PlayerInteractionController InteractionController { get; private set; }
    public static PlayerFlashlight FlashLight { get; private set; }
    public static PlayerCamAnimator CamAnimator { get; private set; }
    public static PlayerSettingsHandler SettingsHandler { get; private set; }

    public int GridFloorId => transform.position.y > 2 ? 1 : 0;
    public float3 Position => transform.position;


    private void Awake()
    {
        PathfindTarget = this;
        Controller = GetComponent<PlayerController>();
        InteractionController = GetComponent<PlayerInteractionController>();
        FlashLight = GetComponent<PlayerFlashlight>();
        CamAnimator = GetComponentInChildren<PlayerCamAnimator>();
        SettingsHandler = GetComponent<PlayerSettingsHandler>();
    }
}