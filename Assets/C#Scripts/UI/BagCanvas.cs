using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagCanvas : MonoBehaviour
{
    public GameObject UsedBagViewOnPanel;
    public GameObject UsedBagView;
    public GameObject UnUsedBagView;
    public void UpdateBagDataOnPanel(List<ItemData> items, BagType bagType)
    {
        for (int i = 0; i < items.Count; i++)
        {
            bool hasItem = items[i].itemImage != null && items[i].itemCount > 0;
            if (bagType == BagType.Used)
            {
                UsedBagViewOnPanel.transform.GetChild(i).GetComponent<BagSlot>().SetItem(items[i].itemImage, items[i].itemCount);
            }
        }
    }
    public void UpdateBagData(List<ItemData> items, BagType bagType)
    {
        for (int i = 0; i < items.Count; i++)
        {
            bool hasItem = items[i].itemImage != null && items[i].itemCount > 0;

            if (bagType == BagType.Used)
            {
                if (UsedBagView.activeSelf)
                {
                    UsedBagView.transform.GetChild(i).GetComponent<BagSlot>().SetItem(items[i].itemImage, items[i].itemCount);
                }
            }
            else
            {
                if (UnUsedBagView.activeSelf)
                {
                    UnUsedBagView.transform.GetChild(i).GetComponent<BagSlot>().SetItem(items[i].itemImage, items[i].itemCount);
                }
            }
        }
    }
}
