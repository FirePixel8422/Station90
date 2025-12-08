using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float popupFadeSpeed = 5;

    [SerializeField] private float interactionCooldown;
    [SerializeField] private bool interactable = true;
    public bool IsInteractable => interactable && Time.time >= lastInteractGlobalTime + interactionCooldown;


    private float lastInteractGlobalTime;

    private bool isPopupActive;
    private Coroutine activePopupFadeCO;



    private void Awake()
    {
        lastInteractGlobalTime = -interactionCooldown;
    }

    private void OnEnable() => InteractionManager.Interactables.Add(this);
    private void OnDisable() => InteractionManager.Interactables.Remove(this);

    public bool TryInteract()
    {
        if (IsInteractable)
        {
            lastInteractGlobalTime = Time.time;
            OnInteract();
            return true;
        }
        return false;
    }
    public void Release()
    {
        OnRelease();
    }
    protected virtual void OnInteract() { }
    protected virtual void OnRelease() { }


    public void SetPopupActiveState(bool newState)
    {
        // Return if newState == currentState
        if (isPopupActive == newState) return;

        // Update value
        isPopupActive = newState;

        if (activePopupFadeCO != null)
        {
            StopCoroutine(activePopupFadeCO);
        }

        float start = newState ? 0f : 1f;
        float end = newState ? 1f : 0f;

        activePopupFadeCO = StartCoroutine(FadePopup(start, end, popupFadeSpeed));
    }

    private IEnumerator FadePopup(float start, float end, float speed)
    {
        Color newCol = popupText.color;
        float startTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            yield return null;

            t = (Time.time - startTime) * popupFadeSpeed;
            newCol.a = math.lerp(start, end, t);

            popupText.color = newCol;
        }
    }
}