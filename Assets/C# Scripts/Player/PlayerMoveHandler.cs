using UnityEngine;


[System.Serializable]
public class PlayerMoveHandler
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float sprintSpeed = 1.5f;

    [Header("Max time sprinting")]
    [SerializeField] private float maxStaminaTime;
    [SerializeField] private float cStaminaTime;

    [Header("Delay before stamina regen and regen speed")]
    [SerializeField] private float staminaRegenDelay;
    [SerializeField] private float staminaRegenSpeed;

    [Header("Smoothing between movement speed states")]
    [SerializeField] private float accelerationSpeed = 5f;

    [HideInInspector] public float CurrentSpeed;
    [HideInInspector] public bool SprintInput;
    [HideInInspector] public bool MoveInput;
    private bool IsSprinting => SprintInput && MoveInput && cStaminaTime > 0;

    private float timeSinceSprinting;


    public void OnUpdate()
    {
        float targetSpeed = IsSprinting ? sprintSpeed : moveSpeed;
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, accelerationSpeed * Time.deltaTime);

        if (IsSprinting)
        {
            cStaminaTime = Mathf.MoveTowards(cStaminaTime, 0, Time.deltaTime);
            timeSinceSprinting = 0;
        }
        else
        {
            if (timeSinceSprinting > staminaRegenDelay)
            {
                cStaminaTime = Mathf.MoveTowards(cStaminaTime, maxStaminaTime, staminaRegenSpeed * Time.deltaTime);
            }

            timeSinceSprinting += Time.deltaTime;
        }
    }
}