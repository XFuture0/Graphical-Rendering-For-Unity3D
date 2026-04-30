using UnityEngine;

public class GenPerlinNoiseMapFromShader : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;
    private int InstanceCount = 10;
    public ComputeShader computeshader;
    private GraphicsBuffer instanceBuffer;
    private GraphicsBuffer IndirectBuffer;
    private int STRIDE = 80;//步幅
    private MaterialPropertyBlock props;
    private void Start()
    {
        StartGenPerlinNoiseMapPer();
    }
    private void Update()
    {
        UpdateCurPart();
    }
    private void StartGenPerlinNoiseMapPer()
    {
        // 实例数据缓冲区
        instanceBuffer = new GraphicsBuffer(
              target: GraphicsBuffer.Target.Structured,
              count: InstanceCount,
              stride: STRIDE
        );
        // 间接参数缓冲区（IndirectArguments）
        IndirectBuffer = new GraphicsBuffer(
            target: GraphicsBuffer.Target.IndirectArguments,
            count: 1, //包含1个参数块（每个块存储5个参数）
            stride: 20 // 5*4字节步幅(args)
        );
        uint[] args = {
            mesh.GetIndexCount(0), // 每个实例的索引数
            (uint)InstanceCount,   // 实例总数
            0, 0, 0                // 起始索引/顶点/实例位置
        };
        IndirectBuffer.SetData(args);
        computeshader.SetBuffer(computeshader.FindKernel("BlockShader"), "_InstanceBuffer", instanceBuffer);
        computeshader.SetInt("_InstanceCount", InstanceCount);
        props = new MaterialPropertyBlock();
        props.SetBuffer("_InstanceBuffer", instanceBuffer);
        computeshader.Dispatch(
            computeshader.FindKernel("BlockShader"),
            Mathf.CeilToInt(InstanceCount / 64f), 1, 1
            );
    }
    private void UpdateCurPart()
    {
        Graphics.DrawMeshInstancedIndirect(
            mesh,
            0,
            mat,
            new Bounds(new Vector3(0,0,0),new Vector3(200,200,200)),
            IndirectBuffer,
            0,
            props
            );
    }
    private void OnDestroy()
    {
        IndirectBuffer?.Release();
        instanceBuffer?.Release();
    }
}
