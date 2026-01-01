using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    public T[] items;
    private int currentItemCount;
    public int Count => currentItemCount;


    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) >= 0) return;

                Swap(item, items[swapIndex]);
            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        int parentGridPos = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentGridPos];

            if (item.CompareTo(parentItem) <= 0) break;

            Swap(item, parentItem);
            parentGridPos = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        (itemB.HeapIndex, itemA.HeapIndex) = (itemA.HeapIndex, itemB.HeapIndex);
    }
}