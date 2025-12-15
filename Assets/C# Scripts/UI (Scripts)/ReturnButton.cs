using UnityEngine.UI;



public class ReturnButton : Button
{
    private static ReturnButton ActiveReturnButton { get; set; }
    

    protected override void OnEnable()
    {
        base.OnEnable();
        ActiveReturnButton = this;
    }

    public static bool PressActiveReturnButton()
    {
        bool buttonActive = ActiveReturnButton != null && ActiveReturnButton.enabled == true;

        if (buttonActive == false)
        {
            ActiveReturnButton = null;
            return false;
        }

        ActiveReturnButton.onClick?.Invoke();
        return true;
    }
}