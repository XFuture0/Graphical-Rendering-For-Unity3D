using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapSlot
{
    public int Seed;
    public Vector3 PlayerStartPosition;
    public MapSlot(int seed)
    {
        Seed = seed;
    }
}
[CreateAssetMenu (menuName = "Data/MapData")]
public class MapData : ScriptableObject
{
    public List<MapSlot> MapSlots;
}
