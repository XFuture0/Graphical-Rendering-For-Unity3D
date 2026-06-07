using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : SingleTons<BagManager>
{
    public BagData UsedBagData;
    public BagData UnUsedBagData;
    public bool AddItem(Sprite sprite)
    {
        if (sprite == null) return false;
        int existingIndex = FindItemIndex(UsedBagData, sprite);
        if (existingIndex != -1)
        {
            UsedBagData.items[existingIndex].itemCount++;
            return true;
        }
        existingIndex = FindItemIndex(UnUsedBagData, sprite);
        if (existingIndex != -1)
        {
            UnUsedBagData.items[existingIndex].itemCount++;
            return true;
        }
        int emptySlot = FindEmptySlot(UsedBagData);
        if (emptySlot != -1)
        {
            UsedBagData.items[emptySlot] = new ItemData
            {
                itemImage = sprite,
                itemCount = 1
            };
            return true;
        }
        emptySlot = FindEmptySlot(UnUsedBagData);
        if (emptySlot != -1)
        {
            UnUsedBagData.items[emptySlot] = new ItemData
            {
                itemImage = sprite,
                itemCount = 1
            };
            return true;
        }
        return false;
    }
    private int FindItemIndex(BagData bagData, Sprite sprite)
    {
        for (int i = 0; i < bagData.items.Count; i++)
        {
            if (bagData.items[i] != null && bagData.items[i].itemImage == sprite && bagData.items[i].itemCount > 0)
            {
                return i;
            }
        }
        return -1;
    }
    private int FindEmptySlot(BagData bagData)
    {
        for (int i = 0; i < bagData.items.Count; i++)
        {
            if (bagData.items[i] == null || bagData.items[i].itemCount <= 0)
            {
                return i;
            }
        }
        return -1;
    }
}
