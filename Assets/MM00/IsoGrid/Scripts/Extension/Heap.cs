using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mixmotion00.Grid.Pathfinding
{
    public class Heap<T> where T : IHeapItem<T>
    {
        T[] items;
        int currentItemCount;

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
            SortUp(item); // G cost improved
        }

        public int Count
        {
            get { return currentItemCount; }
        }

        public bool Contains(T item)
        {
            return item.HeapIndex >= 0 &&
                   item.HeapIndex < currentItemCount &&
                   Equals(items[item.HeapIndex], item);
        }

        void SortDown(T item)
        {
            while (true)
            {
                int leftChildIndex = item.HeapIndex * 2 + 1;
                int rightChildIndex = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (leftChildIndex < currentItemCount)
                {
                    swapIndex = leftChildIndex;

                    if (rightChildIndex < currentItemCount)
                    {
                        if (items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0)
                            swapIndex = rightChildIndex;
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                        Swap(item, items[swapIndex]);
                    else
                        return;
                }
                else
                {
                    return;
                }
            }
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }
                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void Swap(T a, T b)
        {
            items[a.HeapIndex] = b;
            items[b.HeapIndex] = a;

            int aIndex = a.HeapIndex;
            a.HeapIndex = b.HeapIndex;
            b.HeapIndex = aIndex;
        }
    }

}
