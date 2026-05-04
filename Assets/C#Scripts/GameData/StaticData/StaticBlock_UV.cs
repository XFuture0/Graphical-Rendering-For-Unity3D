using UnityEngine;
using System.Collections.Generic;
public enum BlockType
{
    Grass = 0,      
    Dirt = 1,       
    Stone = 2,      
    Sand = 3,       
}
public enum BlockFace
{
    Front = 0,   
    Back = 1,   
    Left = 2,  
    Right = 3,   
    Top = 4,     
    Bottom = 5  
}
[System.Serializable]
public class BlockMaterialConfig
{
    public BlockType BlockType;
    public int[] FaceMaterials; 

    public BlockMaterialConfig(BlockType type, int front, int back, int left, int right, int top, int bottom)
    {
        BlockType = type;
        FaceMaterials = new int[6];
        FaceMaterials[0] = front;
        FaceMaterials[1] = back;
        FaceMaterials[2] = left;
        FaceMaterials[3] = right;
        FaceMaterials[4] = top;
        FaceMaterials[5] = bottom;
    }
}

public static class StaticBlock_UV
{
    private static readonly Dictionary<BlockType, BlockMaterialConfig> BlockMaterialConfigs = new Dictionary<BlockType, BlockMaterialConfig>
    {
        {
            BlockType.Grass,
            new BlockMaterialConfig(BlockType.Grass, 1, 1, 1, 1, 0, 2) 
        },
        {
            BlockType.Dirt,
            new BlockMaterialConfig(BlockType.Dirt, 2, 2, 2, 2, 2, 2) 
        },
        {
            BlockType.Stone,
            new BlockMaterialConfig(BlockType.Stone, 3, 3, 3, 3, 3, 3)
        },
    };
    public static BlockMaterialConfig GetBlockMaterialConfig(BlockType blockType)
    {
        if (BlockMaterialConfigs.TryGetValue(blockType, out var config))
        {
            return config;
        }
        return BlockMaterialConfigs[BlockType.Dirt];
    }
}
