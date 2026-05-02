using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemDrag : MonoBehaviour
{
    public GameObject DragSlot;
    private bool IsDrag;
    private Transform originalSlot;
    private Transform targetSlot;
    private void Update()
    {
        HandleDrag();
    }
    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0) && !IsDrag)
        {
            StartDrag();
            return;
        }

        if (IsDrag && DragSlot != null)
        {
            FollowMouse();
        }
        if (Input.GetMouseButtonDown(0) && IsDrag)
        {
            EndDrag();
        }
    }
    private void StartDrag()
    {
        GameObject slot = GetSlotUnderMouse();
        if (slot != null && slot.GetComponent<Image>().sprite != null)
        {
            DragSlot.transform.position = Input.mousePosition;
            IsDrag = true;
            originalSlot = slot.transform;
            CopyItemData(slot.transform, DragSlot.transform);
            originalSlot.GetComponent<BagSlot>().Clear();
            originalSlot.GetComponent<BagSlot>().SaveItemData();
        }
    }

    private void FollowMouse()
    {
        DragSlot.transform.position = Input.mousePosition;
    }

    private void EndDrag()
    {
        GameObject slot = GetSlotUnderMouse();
        if (slot != null)
        {
            targetSlot = slot.transform;
            CopyItemData(DragSlot.transform, targetSlot.transform);
        }
        else
        {
            targetSlot = originalSlot;
            CopyItemData(DragSlot.transform, targetSlot.transform);
        }
        targetSlot.GetComponent<BagSlot>().SaveItemData();
        IsDrag = false;
        originalSlot = null;
        targetSlot = null;
    }
    private void CopyItemData(Transform source, Transform target)
    {
        BagSlot sourceImage = source.GetComponent<BagSlot>();
        BagSlot targetImage = target.GetComponent<BagSlot>();
        targetImage.SetItem(sourceImage.sprite, sourceImage.Count);
        sourceImage.Clear();
    }
    private GameObject GetSlotUnderMouse()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("ItemSlot"))
            {
                return result.gameObject;
            }
        }
        return null;
    }
}
