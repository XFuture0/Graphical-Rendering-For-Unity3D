using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenPerlinNoiseMap;

public class MapManager : SingleTons<MapManager>
{
    public GenPerlinNoiseMap genPerlinNoiseMap;
    public HashSet<Vector3Int> brokenBlocks = new HashSet<Vector3Int>();
    public HashSet<Vector3Int> placedBlocks = new HashSet<Vector3Int>();

    protected override void Awake()
    {
        base.Awake();
    }
    public void BreakBlocks(Matrix4x4 BlockMatrix)
    {
        Vector3Int pos = Vector3Int.RoundToInt(BlockMatrix.GetPosition());
        brokenBlocks.Add(pos);
        placedBlocks.Remove(pos);
        genPerlinNoiseMap.BreakBlocks(BlockMatrix);
    }
    public void CreateBlocks(Matrix4x4 BlockMatrix)
    {
        Vector3Int pos = Vector3Int.RoundToInt(BlockMatrix.GetPosition());
        placedBlocks.Add(pos);
        brokenBlocks.Remove(pos);
        genPerlinNoiseMap.CreateBlocks(BlockMatrix);
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
}
