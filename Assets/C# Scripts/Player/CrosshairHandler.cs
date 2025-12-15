using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CrosshairHandler
{
    [SerializeField] private float alphaChangeSpeed;
    [SerializeField] private Image crosshairImage;

    private bool isActive;

    private Color crosshairColor;
    private float crosshairBaseAlpha;


    public void Init()
    {
        crosshairColor = crosshairImage.color;
        crosshairBaseAlpha = crosshairColor.a;

        crosshairColor.a = 0;
        crosshairImage.color = crosshairColor;
    }
    public void OnUpdate(bool newActiveState)
    {
        isActive = newActiveState;

        crosshairColor.a = Mathf.MoveTowards(crosshairColor.a, isActive ? 1 : 0, alphaChangeSpeed * Time.deltaTime);
        crosshairImage.color = crosshairColor * crosshairBaseAlpha;
    }
}
