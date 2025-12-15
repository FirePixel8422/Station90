using UnityEngine;



public class PlayerDataLibrary : MonoBehaviour
{
    public static PlayerController Controller {get; private set;}
    public static PlayerInteractionController InteractionController { get; private set; }
    public static PlayerFlashlight FlashLight { get; private set; }
    public static PlayerCamAnimator CamAnimator { get; private set; }


    private void Awake()
    {
        FlashLight = GetComponent<PlayerFlashlight>();
        Controller = GetComponent<PlayerController>();
        CamAnimator = GetComponentInChildren<PlayerCamAnimator>();
    }
}