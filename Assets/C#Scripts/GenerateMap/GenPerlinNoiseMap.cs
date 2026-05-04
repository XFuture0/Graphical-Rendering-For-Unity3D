using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
public enum BlockType
{
    Grass = 0,     
    Dirt = 1,     
    Stone = 2,   
    Sand = 3,    
}
public class PartBlockPro
{
    public int Count;
    public Vector3 PartOffect;
    public Vector2 CombinePart;
    public Mesh PartMesh;
    public Mesh Lod_Top;
    public Mesh Lod_Middle;
    public Bounds PartBound;
    public LodLayer lodLayer;
    public Matrix4x4[] CachedMatrices;
    public PartBlockPro(Vector3 partOffect)
    {
        PartOffect = partOffect;
    }
    public override bool Equals(object obj)
    {
        return obj is PartBlockPro other &&
           PartOffect == other.PartOffect;
    }
    public override int GetHashCode()
    {
        return PartOffect.GetHashCode();
    }
    public void DestroyAllMeshes()
    {
        UnityEngine.Object.Destroy(PartMesh);
        UnityEngine.Object.Destroy(Lod_Top);
        UnityEngine.Object.Destroy(Lod_Middle);
    }
}
public class GenPerlinNoiseMap : MonoBehaviour
{
    public float scale_Mountain;
    public float height_Mountain;
    public Material mat;
    public float ViewDistance;
    public int LayerCount;
    public int Lod1_LayerCount;
    public int Lod2_LayerCount;
    private int seed;
    private Vector3 CurPart;
    public Dictionary<PartBlockPro,List<Matrix4x4>> PartBlocks = new Dictionary<PartBlockPro,List<Matrix4x4>>();
    private List<PartBlockPro> PartBlockDelList = new List<PartBlockPro>();
    public void InitMap(int Seed)
    {
        seed = Seed;
        int height = GetGroundHeightAt(0, 0);
        UIManager.Instance.InitGame(height);
        StartCoroutine(InitGenMap());
    }
    private IEnumerator InitGenMap()
    {
        while(true){
            Vector2 Curpart = new Vector2(CurPart.x, CurPart.z);
            int RoundCount = 1;
            int CurPartCount = 0;
            AddGenPerlinNoiseMapPer(Curpart);
            AddGenPerlinNoiseMapPer(new Vector2(Curpart.x + 1,0));
            yield return new WaitForSeconds(0.2f);
            while (CurPartCount < LayerCount * LayerCount)
            {
                for(int i = 0; i < RoundCount; i++)
                {
                    Curpart -= new Vector2(0, 1);
                    AddGenPerlinNoiseMapPer(Curpart);
                    yield return new WaitForSeconds(0.1f);
                }
                for (int i = 0; i < RoundCount; i++)
                {
                    Curpart -= new Vector2(1, 0);
                    AddGenPerlinNoiseMapPer(Curpart);
                    yield return new WaitForSeconds(0.1f);
                }
                RoundCount++;
                for (int i = 0; i < RoundCount; i++)
                {
                    Curpart += new Vector2(0, 1);
                    AddGenPerlinNoiseMapPer(Curpart);
                    yield return new WaitForSeconds(0.1f);
                }
                for (int i = 0; i < RoundCount; i++)
                {
                    Curpart += new Vector2(1, 0);
                    AddGenPerlinNoiseMapPer(Curpart);
                    yield return new WaitForSeconds(0.1f);
                }
                RoundCount++;
                CurPartCount += 3 * RoundCount - 3;
            }
            foreach (var part in PartBlocks.Keys)
            {
                if (Vector2.Distance(part.CombinePart, Curpart) > 6)
                {
                    part.DestroyAllMeshes();
                    PartBlockDelList.Add(part);
                }
            }
            foreach (var part in PartBlockDelList)
            {
                PartBlocks.Remove(part);
            }
            PartBlockDelList.Clear();
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void Update()
    {
        UpdateCurPart();
        OnGenPerlinNoiseMap();
    }
    private void UpdateCurPart()
    {
        var CameraPosition = Camera.main.transform.position;
        CurPart = new Vector3((int)CameraPosition.x / 50, 0, (int)CameraPosition.z / 50);
        if(CameraPosition.x < 0) CurPart.x--;
        if(CameraPosition.z < 0) CurPart.z--;
    }
    private void OnGenPerlinNoiseMap()
    {
        foreach (var part in PartBlocks)
        {
            var Distance = Vector3.Distance(part.Key.PartOffect, CurPart * 50 + new Vector3(25, 0, 25));
            if (Distance < ViewDistance)
            {
                RefreshPartBlockLodLayer(part.Key, part.Value, CurPart, part.Key.PartOffect);
                Graphics.DrawMeshInstanced(part.Key.PartMesh, 0, mat, part.Key.CachedMatrices, 1);
            }
        }
    }
    private void AddGenPerlinNoiseMapPer(Vector2 AddPart)
    {
        var CheckBlock = new PartBlockPro(new Vector3(50 * AddPart.x + 25, 0, 50 * AddPart.y + 25));
        if(PartBlocks.ContainsKey(CheckBlock)) return;
        int BlockCount = 0;
        List<Matrix4x4> BlockMatrices = new List<Matrix4x4>(50000);
        List<int> BlockTypes = new List<int>(50000);  // 记录每个方块的材质类型

        float seedOffsetX = HashToOffset(seed, 0.001f);
        float seedOffsetY = HashToOffset(seed + 1, 0.001f);

        for (int m = 0; m < 50; m++)
        {
            for (int n = 0; n < 50; n++)
            {
                int MountainHigh = (int)Math.Pow(2,Mathf.PerlinNoise((50 * AddPart.x + m) * scale_Mountain + seedOffsetX, (50 * AddPart.y + n) * scale_Mountain + seedOffsetY) * height_Mountain);

                for (int k = 0; k <= MountainHigh; k++)
                {
                    BlockMatrices.Add(Matrix4x4.TRS(new Vector3(50 * AddPart.x + m, k, 50 * AddPart.y + n), Quaternion.identity, Vector3.one));

                    // 根据高度确定材质类型
                    BlockType blockType = GetBlockTypeByHeight(k, MountainHigh);
                    BlockTypes.Add((int)blockType);

                    BlockCount++;
                }
            }
        }

        var CurBlockPro = new PartBlockPro(new Vector3(50 * AddPart.x + 25, 0, 50 * AddPart.y + 25));
        CurBlockPro.CombinePart = AddPart;
        var CurBlockMatrices = new List<Matrix4x4>(BlockCount);
        CurBlockMatrices.AddRange(BlockMatrices);
        CurBlockPro.Count = BlockCount;
        CurBlockPro.CachedMatrices = CurBlockMatrices.ToArray();

        // 传递 BlockTypes 到 VertexCombine
        CurBlockPro.Lod_Top = VertexCombine(BlockCount, CurBlockMatrices, BlockTypes, AddPart, StaticBlock_Lod_Top.Cube_Vertex, StaticBlock_Lod_Top.Cube_Index, StaticBlock_Lod_Top.Cube_UV);
        CurBlockPro.Lod_Middle = VertexCombine(BlockCount, CurBlockMatrices, BlockTypes, AddPart, StaticBlock_Lod_Middle.Cube_Vertex, StaticBlock_Lod_Middle.Cube_Index, StaticBlock_Lod_Middle.Cube_UV);

        CurBlockPro.lodLayer = LodLayer.NULL;
        CurBlockPro.PartBound = new Bounds(CurBlockPro.PartOffect, new Vector3(50.0f, 20.0f, 50.0f));
        PartBlocks.Add(CurBlockPro, CurBlockMatrices);
    }

    // 根据高度获取方块类型
    private BlockType GetBlockTypeByHeight(int y, int maxHeight)
    {
        if (y == maxHeight)
            return BlockType.Grass;      // 最顶层 - 草地
        else if (y >= maxHeight - 3)
            return BlockType.Dirt;       // 顶层往下3格 - 泥土
        else
            return BlockType.Stone;      // 更深 - 石头
    }
    public int AtlasGridSize = 4;  // 图集网格大小，默认4x4

    private Mesh VertexCombine(int CombineCount, List<Matrix4x4> Transform, List<int> BlockTypeList, Vector2 CombinePart, Vector3[] Cube_Vertex, int[] Cube_Index, Vector2[] Cube_UV)
    {
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = IndexFormat.UInt32;
        NativeArray<Vector3> Position = new NativeArray<Vector3>(CombineCount, Allocator.TempJob);
        NativeArray<int> BlockTypes = new NativeArray<int>(CombineCount, Allocator.TempJob);

        for(int i = 0; i < Transform.Count && i < CombineCount; i++)
        {
            Position[i] = Transform[i].GetPosition() - new Vector3(CombinePart.x,0,CombinePart.y) * 50;
            BlockTypes[i] = BlockTypeList[i];
        }

        NativeArray<Vector2> Mesh_UV = new NativeArray<Vector2>(Cube_UV.Length, Allocator.TempJob);
        NativeArray<Vector3> Mesh_Vertex = new NativeArray<Vector3>(Cube_Vertex.Length, Allocator.TempJob);
        NativeArray<int> Mesh_Index = new NativeArray<int>(Cube_Index.Length, Allocator.TempJob);
        NativeArray<Vector3> CombineVertex = new NativeArray<Vector3>(Cube_Vertex.Length * CombineCount, Allocator.TempJob);
        NativeArray<Vector2> CombineUV = new NativeArray<Vector2>(Cube_UV.Length * CombineCount, Allocator.TempJob);
        NativeArray<int> CombineIndex = new NativeArray<int>(Cube_Index.Length * CombineCount, Allocator.TempJob);
        Mesh_UV.CopyFrom(Cube_UV);
        Mesh_Vertex.CopyFrom(Cube_Vertex);
        Mesh_Index.CopyFrom(Cube_Index);

        CombineMeshJob combineMeshJob = new CombineMeshJob
        {
            Position = Position,
            BlockTypes = BlockTypes,
            Mesh_Vertex = Mesh_Vertex,
            Mesh_UV = Mesh_UV,
            Mesh_Triangles = Mesh_Index,
            CombineCount = CombineCount,
            CombinesVertex = CombineVertex,
            CombineUV = CombineUV,
            CombinesIndex = CombineIndex,
            AtlasGridSize = AtlasGridSize
        };

        var CombineMeshJobHandle = combineMeshJob.Schedule();
        CombineMeshJobHandle.Complete();
        newMesh.vertices = CombineVertex.ToArray();
        newMesh.triangles = CombineIndex.ToArray();
        newMesh.uv = CombineUV.ToArray();

        Position.Dispose();
        BlockTypes.Dispose();
        Mesh_UV.Dispose();
        Mesh_Vertex.Dispose();
        Mesh_Index.Dispose();
        CombineVertex.Dispose();
        CombineUV.Dispose();
        CombineIndex.Dispose();

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        return newMesh;
    }
    private void RefreshPartBlockLodLayer(PartBlockPro PartBlockPro, List<Matrix4x4> Transform, Vector3 CurPart, Vector3 PartOffect)
    {
        Vector3 PartOffect_Normal = new Vector3(PartBlockPro.CombinePart.x, 0, PartBlockPro.CombinePart.y);
        var LodDistance = math.abs(PartOffect_Normal.x - CurPart.x) > math.abs(PartOffect_Normal.z - CurPart.z) ? math.abs(PartOffect_Normal.x - CurPart.x) : math.abs(PartOffect_Normal.z - CurPart.z);
        bool IsOffectX = math.abs(PartOffect_Normal.x - CurPart.x) > math.abs(PartOffect_Normal.z - CurPart.z);
        if (PartOffect_Normal.x > CurPart.x && IsOffectX || PartOffect_Normal.z >CurPart.z && !IsOffectX) LodDistance++;
        if (LodDistance <= Lod1_LayerCount && PartBlockPro.lodLayer != LodLayer.Lod_Top)
        {
            if (PartBlockPro.PartMesh != null)
            {
                Destroy(PartBlockPro.PartMesh);
            }
            PartBlockPro.lodLayer = LodLayer.Lod_Top;
            PartBlockPro.PartMesh = PartBlockPro.Lod_Top;
        }
        else if(LodDistance > Lod1_LayerCount && LodDistance <= Lod2_LayerCount && PartBlockPro.lodLayer != LodLayer.Lod_Middle)
        {
            if (PartBlockPro.PartMesh != null)
            {
                Destroy(PartBlockPro.PartMesh);
            }
            PartBlockPro.lodLayer = LodLayer.Lod_Middle;
            PartBlockPro.PartMesh = PartBlockPro.Lod_Middle;
        }
        else if(LodDistance > Lod2_LayerCount && PartBlockPro.lodLayer != LodLayer.Lod_Bottom)
        {
            if (PartBlockPro.PartMesh != null)
            {
                Destroy(PartBlockPro.PartMesh);
            }
            PartBlockPro.lodLayer = LodLayer.Lod_Bottom;
            PartBlockPro.PartMesh = null;
        }
    }
    private void RefreshCurBlocks(Vector3 BreakPart,bool IsCreate)
    {
        var CurBlockPro = new PartBlockPro(new Vector3(BreakPart.x,0,BreakPart.z) * 50 + new Vector3(25, 0, 25));
        foreach (var PartBlockPro_Key in PartBlocks.Keys)
        {
            if(PartBlockPro_Key.PartOffect == CurBlockPro.PartOffect)
            {
                if(IsCreate) PartBlockPro_Key.Count++;
                else if(!IsCreate) PartBlockPro_Key.Count--;
                if (PartBlockPro_Key.Lod_Top != null)
                {
                    Destroy(PartBlockPro_Key.Lod_Top);
                }
                if (PartBlockPro_Key.Lod_Middle != null)
                {
                    Destroy(PartBlockPro_Key.Lod_Middle);
                }
                if (PartBlockPro_Key.PartMesh != null)
                {
                    Destroy(PartBlockPro_Key.PartMesh);
                }

                // 重新计算 BlockTypes
                List<int> blockTypes = CalculateBlockTypes(PartBlocks[PartBlockPro_Key]);

                PartBlockPro_Key.Lod_Top = VertexCombine(PartBlockPro_Key.Count, PartBlocks[PartBlockPro_Key], blockTypes, PartBlockPro_Key.CombinePart, StaticBlock_Lod_Top.Cube_Vertex, StaticBlock_Lod_Top.Cube_Index, StaticBlock_Lod_Top.Cube_UV);
                PartBlockPro_Key.Lod_Middle = VertexCombine(PartBlockPro_Key.Count, PartBlocks[PartBlockPro_Key], blockTypes, PartBlockPro_Key.CombinePart, StaticBlock_Lod_Middle.Cube_Vertex, StaticBlock_Lod_Middle.Cube_Index, StaticBlock_Lod_Middle.Cube_UV);
                PartBlockPro_Key.lodLayer = LodLayer.NULL;
            }
        }
    }

    // 根据方块位置列表计算材质类型
    private List<int> CalculateBlockTypes(List<Matrix4x4> matrices)
    {
        List<int> blockTypes = new List<int>(matrices.Count);

        foreach (var matrix in matrices)
        {
            Vector3 pos = matrix.GetPosition();
            int y = Mathf.FloorToInt(pos.y);

            // 使用 PerlinNoise 重新计算该位置的最大高度
            float seedOffsetX = HashToOffset(seed, 0.001f);
            float seedOffsetY = HashToOffset(seed + 1, 0.001f);
            float noiseValue = (float)Math.Pow(2, Mathf.PerlinNoise(pos.x * scale_Mountain + seedOffsetX, pos.z * scale_Mountain + seedOffsetY) * height_Mountain);
            int maxHeight = Mathf.FloorToInt(noiseValue);

            BlockType blockType = GetBlockTypeByHeight(y, maxHeight);
            blockTypes.Add((int)blockType);
        }

        return blockTypes;
    }
    public void BreakBlocks(Matrix4x4 BlockMatrix)
    {
        Vector3 BreakPart = new Vector3((int)(BlockMatrix.GetPosition().x / 50),0,(int)(BlockMatrix.GetPosition().z / 50));
        if(BlockMatrix.GetPosition().x < 0) BreakPart.x--;
        if(BlockMatrix.GetPosition().z < 0) BreakPart.z--;
        var BreakBlockPro = new PartBlockPro(new Vector3(BreakPart.x,0,BreakPart.z) * 50 + new Vector3(25, 0, 25));
        PartBlocks[BreakBlockPro].Remove(BlockMatrix);
        RefreshCurBlocks(BreakPart, false);
    }
    public void CreateBlocks(Matrix4x4 BlockMatrix)
    {
        Vector3 BreakPart = new Vector3((int)(BlockMatrix.GetPosition().x / 50), 0, (int)(BlockMatrix.GetPosition().z / 50));
        if (BlockMatrix.GetPosition().x < 0) BreakPart.x--;
        if (BlockMatrix.GetPosition().z < 0) BreakPart.z--;
        var BreakBlockPro = new PartBlockPro(new Vector3(BreakPart.x, 0, BreakPart.z) * 50 + new Vector3(25, 0, 25));
        PartBlocks[BreakBlockPro].Add(BlockMatrix);
        RefreshCurBlocks(BreakPart, true);
    }
    private float HashToOffset(int seed, float multiplier)
    {
        uint s = (uint)seed;
        s ^= s << 13;
        s ^= s >> 17;
        s ^= s << 5;
        return s % 100000 * multiplier;
    }
    public bool HasBlockAt(Vector3Int pos)
    {
        float seedOffsetX = HashToOffset(seed, 0.001f);
        float seedOffsetY = HashToOffset(seed + 1, 0.001f);
        float noiseValue = (float)Math.Pow(2, Mathf.PerlinNoise(pos.x * scale_Mountain + seedOffsetX, pos.z * scale_Mountain + seedOffsetY) * height_Mountain);
        int groundHeight = Mathf.FloorToInt(noiseValue);
        return pos.y <= groundHeight;
    }
    public int GetGroundHeightAt(float x, float z)
    {
        float seedOffsetX = HashToOffset(seed, 0.001f);
        float seedOffsetY = HashToOffset(seed + 1, 0.001f);
        float noiseValue = (float)Math.Pow(2, Mathf.PerlinNoise(x * scale_Mountain + seedOffsetX, z * scale_Mountain + seedOffsetY) * height_Mountain);
        return Mathf.FloorToInt(noiseValue) + 1;
    }
}
[BurstCompile]
public struct CombineMeshJob : IJob
{
    public NativeArray<Vector3> Position;
    public NativeArray<int> BlockTypes;     
    public int CombineCount;
    public NativeArray<Vector3> Mesh_Vertex;
    public NativeArray<Vector2> Mesh_UV;
    public NativeArray<int> Mesh_Triangles;
    public NativeArray<Vector3> CombinesVertex;
    public NativeArray<Vector2> CombineUV;
    public NativeArray<int> CombinesIndex;
    public int AtlasGridSize;           

    public void Execute()
    {
        int TotalVertices = 0;
        int TotalIndex = 0;
        float cellSize = 1.0f / AtlasGridSize;  

        for (int i = 0; i < CombineCount; i++)
        {
            // 获取当前方块的材质类型
            int blockType = BlockTypes[i];
            int atlasX = blockType % AtlasGridSize;
            int atlasY = blockType / AtlasGridSize;

            for(int j = 0;j < Mesh_Vertex.Length; j++)
            {
                CombinesVertex[TotalVertices + j] = Mesh_Vertex[j] + Position[i];

                // 计算图集UV
                float2 baseUV = new float2(Mesh_UV[j].x, Mesh_UV[j].y);
                CombineUV[TotalVertices + j] = new Vector2(
                    baseUV.x * cellSize + atlasX * cellSize,
                    baseUV.y * cellSize + atlasY * cellSize
                );
            }
            for (int j = 0; j < Mesh_Triangles.Length; j++)
            {
                CombinesIndex[TotalIndex + j] = Mesh_Triangles[j] + Mesh_Vertex.Length * i;
            }
            TotalIndex += Mesh_Triangles.Length;
            TotalVertices += Mesh_Vertex.Length;
        }
    }
}
