using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureAtlasGenerator : EditorWindow
{
    [Header("输入纹理")]
    public Texture2D[] textures = new Texture2D[0];

    [Header("图集设置")]
    public int atlasSize = 512;
    public int gridSize = 4;
    public FilterMode filterMode = FilterMode.Point;
    public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

    [Header("保存路径")]
    public string saveFolder = "Assets/Textures";
    public string fileName = "BlockAtlas.png";

    private Vector2 scrollPosition;

    [MenuItem("Tools/Texture Atlas Generator")]
    public static void ShowWindow()
    {
        GetWindow<TextureAtlasGenerator>("Texture Atlas Generator");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("纹理图集生成器", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 纹理列表
        EditorGUILayout.LabelField("输入纹理 (按顺序排列):", EditorStyles.boldLabel);
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty texturesProperty = so.FindProperty("textures");
        EditorGUILayout.PropertyField(texturesProperty, true);
        so.ApplyModifiedProperties();

        GUILayout.Space(10);

        // 图集设置
        EditorGUILayout.LabelField("图集设置:", EditorStyles.boldLabel);
        atlasSize = EditorGUILayout.IntField("图集大小", atlasSize);
        gridSize = EditorGUILayout.IntField("网格大小 (NxN)", gridSize);
        filterMode = (FilterMode)EditorGUILayout.EnumPopup("过滤模式", filterMode);
        wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("环绕模式", wrapMode);

        GUILayout.Space(10);

        // 保存路径
        EditorGUILayout.LabelField("保存设置:", EditorStyles.boldLabel);
        saveFolder = EditorGUILayout.TextField("保存文件夹", saveFolder);
        fileName = EditorGUILayout.TextField("文件名", fileName);

        GUILayout.Space(20);

        // 预览信息
        int cellSize = atlasSize / gridSize;
        int maxTextures = gridSize * gridSize;
        EditorGUILayout.HelpBox(
            $"每个格子大小: {cellSize}x{cellSize}像素\n" +
            $"最大纹理数量: {maxTextures}\n" +
            $"当前纹理数量: {textures.Length}\n" +
            $"保存路径: {saveFolder}/{fileName}",
            MessageType.Info);

        GUILayout.Space(10);

        // 生成按钮
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("生成纹理图集", GUILayout.Height(40)))
        {
            GenerateAtlas();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndScrollView();
    }

    private void GenerateAtlas()
    {
        // 验证输入
        if (textures.Length == 0)
        {
            EditorUtility.DisplayDialog("错误", "请至少添加一张纹理!", "确定");
            return;
        }

        int maxTextures = gridSize * gridSize;
        if (textures.Length > maxTextures)
        {
            EditorUtility.DisplayDialog("错误", $"纹理数量超过网格容量! 最大支持 {maxTextures} 张纹理。", "确定");
            return;
        }

        // 确保文件夹存在
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string fullPath = Path.Combine(saveFolder, fileName);

        // 创建图集
        Texture2D atlas = new Texture2D(atlasSize, atlasSize, TextureFormat.RGBA32, true);
        int cellSize = atlasSize / gridSize;

        // 填充背景为透明
        Color[] clearColors = new Color[atlasSize * atlasSize];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.clear;
        }
        atlas.SetPixels(clearColors);

        // 合并纹理
        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] == null)
            {
                Debug.LogWarning($"第 {i + 1} 个纹理为空，跳过");
                continue;
            }

            int x = i % gridSize;
            int y = i / gridSize;

            // 读取纹理并缩放
            Texture2D scaledTexture = GetReadableTexture(textures[i], cellSize, cellSize);

            // 复制到图集
            atlas.SetPixels(x * cellSize, y * cellSize, cellSize, cellSize, scaledTexture.GetPixels());

            Debug.Log($"添加纹理 [{i}]: {textures[i].name} 到位置 ({x}, {y})");
        }

        atlas.Apply(true);

        // 保存文件
        byte[] bytes = atlas.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);

        // 清理临时对象
        DestroyImmediate(atlas);

        AssetDatabase.Refresh();

        // 配置导入设置
        ConfigureTextureImportSettings(fullPath);

        EditorUtility.DisplayDialog("完成", $"纹理图集已生成!\n路径: {fullPath}\n包含 {textures.Length} 张纹理", "确定");
        Debug.Log($"纹理图集生成完成: {fullPath}");
    }

    private Texture2D GetReadableTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        // 首先尝试直接获取像素（如果纹理已标记为可读）
        try
        {
            Color[] pixels = source.GetPixels();
            if (pixels != null && pixels.Length > 0)
            {
                // 纹理已经是可读的，直接使用
                if (source.width == targetWidth && source.height == targetHeight)
                {
                    return source;
                }
                // 需要缩放
                return ScaleTexture(source, targetWidth, targetHeight);
            }
        }
        catch
        {
            // 纹理不可读，继续下面的方法
        }

        // 使用 RenderTexture 方法（修正颜色空间）
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        // 缩放到目标大小
        if (readableTexture.width != targetWidth || readableTexture.height != targetHeight)
        {
            Texture2D scaled = ScaleTexture(readableTexture, targetWidth, targetHeight);
            DestroyImmediate(readableTexture);
            return scaled;
        }

        return readableTexture;
    }

    private Texture2D ScaleTexture(Texture2D source, int width, int height)
    {
        source.filterMode = filterMode;
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        rt.filterMode = filterMode;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    private void ConfigureTextureImportSettings(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.filterMode = filterMode;
            importer.wrapMode = wrapMode;
            importer.mipmapEnabled = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            AssetDatabase.ImportAsset(path);
            Debug.Log("已配置纹理导入设置");
        }
    }
}
