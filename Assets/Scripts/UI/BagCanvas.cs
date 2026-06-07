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
    public GameObject PlayerBag;
    public GameObject SelectSlot;
    private void OnEnable()
    {
        EventMgr.Instance.AddEventListener<bool>(EventType.SetPlayerBagActive, SetPlayerBagActive);
        EventMgr.Instance.AddEventListener<int>(EventType.HandleSelectSlot,HandleSelectSlot);
    }
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
    public void SetPlayerBagActive(bool isActive)
    {
        PlayerBag.SetActive(isActive);
    }
    public void HandleSelectSlot(int number)
    {
        var rect = SelectSlot.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(720 + (number - 1) * 140, rect.anchoredPosition.y);
    }
    private void OnDisable()
    {
        if(EventMgr.Instance != null)
        {
            EventMgr.Instance.RemoveEventListener<bool>(EventType.SetPlayerBagActive, SetPlayerBagActive);
            EventMgr.Instance.RemoveEventListener<int>(EventType.HandleSelectSlot,HandleSelectSlot);
        }
    }
}
