using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum BagType
{
    Used,
    UnUsed
}
public class BagDataController : MonoBehaviour
{
    public BagData UsedBagData;
    public BagData UnUsedBagData;
    private BagCanvas views;
    private void Awake()
    {
        views = GetComponent<BagCanvas>();
    }
    private void Update()
    {
        views.UpdateBagDataOnPanel(UsedBagData.items, BagType.Used);
        if(UIManager.Instance.PlayerBag.activeSelf)
        {
            SyncToAllViews();
        }
    }
    public void SyncToAllViews()
    {
        views.UpdateBagData(UsedBagData.items, BagType.Used);
        views.UpdateBagData(UnUsedBagData.items, BagType.UnUsed);
    }
    public void SaveItemData(int slotIndex, Sprite sprite, int count, BagType bagType)
    {
        if (bagType == BagType.Used)
        {
            UsedBagData.items[slotIndex].itemImage = sprite;
            UsedBagData.items[slotIndex].itemCount = count;
        }
        else
        {
            UnUsedBagData.items[slotIndex].itemImage = sprite;
            UnUsedBagData.items[slotIndex].itemCount = count;
        }
    }
}
