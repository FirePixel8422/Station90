using UnityEngine;


[System.Serializable]
public class PlayerMoveHandler
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float sprintSpeed = 1.5f;

    [SerializeField] private float maxStamina;
    [SerializeField] private float cStamina;

    [SerializeField] private float staminaRegenDelay;
    [SerializeField] private float staminaRegenSpeed;

    [SerializeField] private float accelerationSpeed = 5f;

    public float CurrentSpeed;
    public bool SprintInput;
    private bool IsSprinting => SprintInput && cStamina > 0;

    private float timeSinceSprinting;


    public void OnUpdate()
    {
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, IsSprinting ? sprintSpeed : moveSpeed, accelerationSpeed * Time.deltaTime);

        if (IsSprinting)
        {
            timeSinceSprinting = 0;
        }
        else if (timeSinceSprinting > staminaRegenDelay)
        {
            cStamina = Mathf.MoveTowards(cStamina, maxStamina, staminaRegenSpeed * Time.deltaTime);
        }
    }
}