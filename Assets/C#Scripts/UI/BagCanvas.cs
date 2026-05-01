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
    public void UpdateBagData(List<ItemData> items, BagType bagType)
    {
        for (int i = 0; i < items.Count; i++)
        {
            bool hasItem = items[i].itemImage != null && items[i].itemCount > 0;

            if (bagType == BagType.Used)
            {
                UpdateSlot(UsedBagViewOnPanel.transform.GetChild(i), items[i], hasItem);
                if (UsedBagView.activeSelf)
                {
                    UpdateSlot(UsedBagView.transform.GetChild(i), items[i], hasItem);
                }
            }
            else
            {
                if (UnUsedBagView.activeSelf)
                {
                    UpdateSlot(UnUsedBagView.transform.GetChild(i), items[i], hasItem);
                }
            }
        }
    }
    private void UpdateSlot(Transform slot, ItemData item, bool hasItem)
    {
        Image itemImage = slot.GetComponent<Image>();
        TextMeshProUGUI itemText = slot.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (hasItem)
        {
            itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1f);
            itemImage.sprite = item.itemImage;
            itemText.text = item.itemCount.ToString();
        }
        else
        {
            itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0f);
            itemText.text = "";
        }
    }
}
