using UnityEngine;

public static class StaticBlock_Lod_Top
{
    public static Vector3[] Cube_Vertex =
    {
        new Vector3(0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, 0.50f),
        new Vector3(0.50f, 0.50f, 0.50f),
        new Vector3(-0.50f, 0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, -0.50f, -0.50f),
        new Vector3(-0.50f, 0.50f, -0.50f),
        new Vector3(0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, -0.50f, -0.50f),
        new Vector3(-0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, 0.50f, 0.50f),
        new Vector3(0.50f, -0.50f, 0.50f),
        new Vector3(0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, 0.50f, 0.50f),
        new Vector3(0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, 0.50f, -0.50f),
        new Vector3(0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, 0.50f, 0.50f),
        new Vector3(0.50f, 0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, 0.50f),
        new Vector3(0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, -0.50f, -0.50f),
    };
    public static int[] Cube_Index =
    {
        0, 2, 1,
        2, 3, 1,
        4, 6, 5,
        6, 7, 5,
        9, 10, 8,
        9, 11, 10,
        13, 14, 12,
        13, 15, 14,
        16, 18, 17,
        18, 19, 17,
        21, 22, 20,
        21, 23, 22
    };
    public static Vector2[] Cube_UV =
    {
        new Vector2(1.0f, 0.0f),  
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 1.0f),  
        new Vector2(0.0f, 1.0f), 
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(0.0f, 1.0f),  
        new Vector2(1.0f, 1.0f),  
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f),  
        new Vector2(0.0f, 1.0f),  
        new Vector2(1.0f, 1.0f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(0.0f, 0.0f),  
        new Vector2(1.0f, 1.0f),  
        new Vector2(0.0f, 1.0f), 
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f),  
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),  
        new Vector2(0.0f, 0.0f),  
        new Vector2(1.0f, 0.0f), 
        new Vector2(0.0f, 1.0f),  
        new Vector2(1.0f, 1.0f),  
    };
}