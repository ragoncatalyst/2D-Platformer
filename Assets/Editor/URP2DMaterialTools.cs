#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class URP2DMaterialTools
{
    const string LitShader = "Universal Render Pipeline/2D/Sprite-Lit-Default";
    const string UnlitShader = "Universal Render Pipeline/2D/Sprite-Unlit-Default";
    const string LitMatPath = "Assets/Materials/SpriteLit2D.mat";
    const string UnlitMatPath = "Assets/Materials/SpriteUnlit2D.mat";

    [MenuItem("Tools/URP 2D/Assign Sprite-Lit-Default To Selection")]
    public static void AssignLitToSelection() => AssignToSelection(LitMatPath, LitShader);

    [MenuItem("Tools/URP 2D/Assign Sprite-Unlit-Default To Selection")]
    public static void AssignUnlitToSelection() => AssignToSelection(UnlitMatPath, UnlitShader);

    static void AssignToSelection(string materialPath, string shaderName)
    {
        var mat = GetOrCreateMaterial(materialPath, shaderName);
        if (mat == null)
        {
            EditorUtility.DisplayDialog("URP 2D", $"Shader not found: {shaderName}. Make sure URP is installed and 2D Renderer is available.", "OK");
            return;
        }

        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0)
        {
            EditorUtility.DisplayDialog("URP 2D", "No GameObjects selected. Select objects in the Hierarchy and try again.", "OK");
            return;
        }

        Undo.RecordObjects(selection, "Assign URP 2D Material");
        int count = 0;
        foreach (var go in selection)
        {
            if (go == null) continue;
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
            {
                // 只处理 SpriteRenderer 和 TilemapRenderer
                if (r is SpriteRenderer || r.GetType().Name == "TilemapRenderer")
                {
                    r.sharedMaterial = mat;
                    count++;
                }
            }
        }
        Debug.Log($"URP2DMaterialTools: Assigned '{mat.name}' to {count} renderers.");
    }

    static Material GetOrCreateMaterial(string path, string shaderName)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        var shader = Shader.Find(shaderName);
        if (shader == null) return null;

        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        mat = new Material(shader) { name = System.IO.Path.GetFileNameWithoutExtension(path) };
        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return mat;
    }
}
#endif
