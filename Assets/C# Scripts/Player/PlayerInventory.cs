



[System.Serializable]
public class PlayerInventory
{
    public static PlayerInventory Instance { get; private set; }


    public void Init()
    {
        Instance = this;
    }
}