using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagSlot : MonoBehaviour
{
    public int slotIndex;
    public int Count;
    public Sprite sprite;
    private Image itemImage;
    private TextMeshProUGUI itemCountText;
    public BagDataController bagDataController;
    public BagType bagType;
    private void Awake()
    {
        itemImage = GetComponent<Image>();
        itemCountText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetItem(Sprite sprite, int count)
    {
        Count = count;
        this.sprite = sprite;
        if (itemImage != null)
        {
            itemImage.sprite = sprite;
            itemImage.color = Count > 0 ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        }
        if (itemCountText != null)
        {
            itemCountText.text = count > 0 ? count.ToString() : "";
        }
    }
    public void SaveItemData()
    {
        bagDataController.SaveItemData(slotIndex, sprite, Count, bagType);
    }
    public void Clear()
    {
        SetItem(null, 0);
    }
}
