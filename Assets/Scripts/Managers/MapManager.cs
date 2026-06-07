using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingleTons<MapManager>
{
    public GenPerlinNoiseMap genPerlinNoiseMap;
    public HashSet<Vector3Int> brokenBlocks = new HashSet<Vector3Int>();
    public HashSet<Vector3Int> placedBlocks = new HashSet<Vector3Int>();
    public GameObject StoneBlock;
    public MapData mapData;
    private int Seed;
    public void BreakBlocks(Matrix4x4 BlockMatrix)
    {
        Vector3Int pos = Vector3Int.RoundToInt(BlockMatrix.GetPosition());
        brokenBlocks.Add(pos);
        placedBlocks.Remove(pos);
        genPerlinNoiseMap.BreakBlocks(BlockMatrix);
        Instantiate(StoneBlock, pos, Quaternion.identity);
    }
    public void CreateBlocks(Matrix4x4 BlockMatrix, int selectedSlot)
    {
        Vector3Int pos = Vector3Int.RoundToInt(BlockMatrix.GetPosition());
        placedBlocks.Add(pos);
        brokenBlocks.Remove(pos);
        if(BagManager.Instance.UsedBagData.items[selectedSlot - 1] != null && BagManager.Instance.UsedBagData.items[selectedSlot - 1].itemCount > 0)
        {
            BagManager.Instance.UsedBagData.items[selectedSlot - 1].itemCount--;
            genPerlinNoiseMap.CreateBlocks(BlockMatrix);
        }
    }
    public bool HasBlockAt(Vector3Int pos)
    {
        if (brokenBlocks.Contains(pos))
        {
            return false;
        }
        if (placedBlocks.Contains(pos))
        {
            return true;
        }
        return genPerlinNoiseMap.HasBlockAt(pos);
    }
    public void SetSeed(int Seed)
    {
        this.Seed = Seed;
    }
    public bool InitMap()
    {
        if(Seed != 0)
        {
            genPerlinNoiseMap.InitMap(Seed);
            return true;
        }
        return false;
    }
    public void DeleteMap()
    {
        if(Seed != 0)
        {
            for(int i = 0; i < mapData.MapSlots.Count; i++)
            {
                if(mapData.MapSlots[i].Seed == Seed)
                {
                    mapData.MapSlots.RemoveAt(i);
                    break;
                }
            }
        }
        DataManager.Instance.SaveData();
    }
}
