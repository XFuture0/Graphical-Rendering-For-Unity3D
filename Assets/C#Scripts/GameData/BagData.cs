using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemData
{
    public Sprite itemImage;
    public int itemCount;
}
[CreateAssetMenu(menuName = "Data/BagData")]
public class BagData : ScriptableObject
{
    public List<ItemData> items;
}
