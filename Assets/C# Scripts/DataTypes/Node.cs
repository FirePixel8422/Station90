


[System.Serializable]
public struct Node
{
    public bool Walkable;
    public int MovePenalty;

    public int GCost;
    public int HCost;

    public int FCost => GCost + HCost;
}