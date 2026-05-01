using System.Collections;
using System.Collections.Generic;
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
        SyncToAllViews();
    }
    public void SyncToAllViews()
    {
        views.UpdateBagData(UsedBagData.items, BagType.Used);
        views.UpdateBagData(UnUsedBagData.items, BagType.UnUsed);
    }
}
