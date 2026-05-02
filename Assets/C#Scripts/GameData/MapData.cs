using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapSlot
{
    public int Index;
    public int Seed;
    public Vector3 PlayerStartPosition;
}
[CreateAssetMenu (menuName = "Data/MapData")]
public class MapData : ScriptableObject
{
    public List<MapSlot> MapSlots;
}
