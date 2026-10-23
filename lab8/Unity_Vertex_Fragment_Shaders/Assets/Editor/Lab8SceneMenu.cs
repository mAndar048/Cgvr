using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Lab8SceneMenu
{
    private const string Path = "Assets/Scenes/Lab8_VertexFragmentShaders.unity";

    [MenuItem("CGVR Labs/Lab 8 - Open Vertex and Fragment Shader Demo")]
    private static void OpenLab8()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(Path);
    }

    [MenuItem("CGVR Labs/Lab 8 - Validate Scene and Shader")]
    private static void Validate()
    {
        bool ok = AssetDatabase.LoadAssetAtPath<SceneAsset>(Path) != null &&
                  AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shaders/Lab8VertexFragmentRGB.shader") != null;
        if (ok) Debug.Log("Lab 8: scene and RGB shader assets found. Press Play to test rendering.");
        else Debug.LogError("Lab 8: missing scene or shader asset.");
    }
}
