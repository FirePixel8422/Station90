


public class NodeHeap
{
    public Node[] items;
    private int currentItemCount;
    public int Count => currentItemCount;


    public NodeHeap(int maxHeapSize)
    {
        items = new Node[maxHeapSize];
    }

    public void Clear()
    {
        currentItemCount = 0;
    }

    public void Add(Node item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }
    public Node RemoveFirst()
    {
        Node firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    public void UpdateItem(Node item)
    {
        SortUp(item);
    }

    public bool Contains(Node item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortDown(Node item)
    {
        int childIndexLeft;
        int childIndexRight;
        int swapIndex;

        while (true)
        {
            childIndexLeft = item.HeapIndex * 2 + 1;
            childIndexRight = item.HeapIndex * 2 + 2;

            if (childIndexLeft >= currentItemCount)
                return;

            swapIndex = childIndexLeft;

            if (childIndexRight < currentItemCount)
            {
                if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                {
                    swapIndex = childIndexRight;
                }
            }

            if (item.CompareTo(items[swapIndex]) >= 0) 
                return;

            Swap(item, items[swapIndex]);
        }
    }

    private void SortUp(Node item)
    {
        int parentGridPos = (item.HeapIndex - 1) / 2;
        Node parentItem;

        while (true)
        {
            parentItem = items[parentGridPos];

            if (item.CompareTo(parentItem) <= 0) break;

            Swap(item, parentItem);
            parentGridPos = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(Node itemA, Node itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        (itemB.HeapIndex, itemA.HeapIndex) = (itemA.HeapIndex, itemB.HeapIndex);
    }
}