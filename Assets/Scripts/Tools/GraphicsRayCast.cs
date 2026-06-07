using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlockGraphicsRayCastHit
{
    public float Distance = 999;
    public Matrix4x4 Position;
}
public static class GraphicsRayCast
{
    public static bool TryBlockGraphicsRayCast(Ray ray, List<Matrix4x4> Vertexs, out BlockGraphicsRayCastHit hit)
    {
        hit = new BlockGraphicsRayCastHit();
        if (Vertexs == null)
        {
            return false;
        }
        float[] CastDistances = new float[100];
        int[] CastIndexs = new int[100];
        float Curdistance = 0;
        int CastCount = 0;
        for(int i =0; i < Vertexs.Count; i++)
        {
            hit.Position = Vertexs[i];
            Bounds bound = new Bounds(hit.Position.GetPosition(), new Vector3(1.0f, 1.0f, 1.0f));
            if(bound.IntersectRay(ray,out Curdistance))
            {
                CastDistances[CastCount] = Curdistance;
                CastIndexs[CastCount] = i;
                CastCount++;
            }
        }
        int CastMin = int.MaxValue;
        for(int i = 0; i < CastCount; i++)
        {
            if(CastDistances[i] < hit.Distance)
            {
                hit.Distance = CastDistances[i];
                CastMin = CastIndexs[i];
            }
        }
        if(CastMin != int.MaxValue)
        {
            hit.Position = Vertexs[CastMin];
            return true;
        }
        return false;
    }
    public static List<Matrix4x4> GetRayCastPartBlocks(Ray ray, Dictionary<PartBlockPro, List<Matrix4x4>> PartBlocks)
    {
        float[] CastDistances = new float[100];
        List<Matrix4x4>[] CastMartexs = new List<Matrix4x4>[100];
        List<Matrix4x4> GetCastMartexs = new List<Matrix4x4>();
        int CastCount = 0;
        int GetCount = 0;
        foreach (var PartBlockPro_Key in PartBlocks.Keys)
        {
            if (PartBlockPro_Key.PartBound.IntersectRay(ray, out float distance))
            {
                CastDistances[CastCount] = distance;
                CastMartexs[CastCount] = PartBlocks[PartBlockPro_Key];
                CastCount++;
            }
        }
        for(int i = 0;i < 3; i++)
        {
            float MinDistance = int.MaxValue;
            int MinIndex = int.MaxValue;
            for (int j = 0; j < CastCount; j++)
            {
                if (CastDistances[j] < MinDistance)
                {
                    MinDistance = CastDistances[j];
                    MinIndex = j;
                }
            }
            if(MinIndex != int.MaxValue)
            {
                CastDistances[MinIndex] = int.MaxValue;
                GetCastMartexs.AddRange(CastMartexs[MinIndex]);
                GetCount++;
            }
        }
        if(GetCount > 0)
        {
            return GetCastMartexs;
        }
        return null;
    }
}
