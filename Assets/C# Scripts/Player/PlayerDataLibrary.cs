using UnityEngine;



public class PlayerDataLibrary : MonoBehaviour
{
    public static PlayerController Controller { get; private set; }
    public static PlayerInteractionController InteractionController { get; private set; }
    public static PlayerFlashlight FlashLight { get; private set; }
    public static PlayerCamAnimator CamAnimator { get; private set; }


    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        InteractionController = GetComponent<PlayerInteractionController>();
        FlashLight = GetComponent<PlayerFlashlight>();
        CamAnimator = GetComponentInChildren<PlayerCamAnimator>();
    }
}