using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Opens the supplied scene on first import; rebuilds it if missing.</summary>
[InitializeOnLoad]
public static class CIE3SceneBuilder
{
    private const string ScenePath = "Assets/Scenes/CIE3_Retail_Furniture_Visualizer.unity";
    private const string OpenedKey = "CIE3.FurnitureSceneOpened";

    static CIE3SceneBuilder() { EditorApplication.delayCall += OpenOnFirstImport; }

    private static void OpenOnFirstImport()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isBatchMode) return;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += OpenOnFirstImport;
            return;
        }
        if (SessionState.GetBool(OpenedKey, false)) return;
        SessionState.SetBool(OpenedKey, true);
        // Never discard an existing scene edit on a domain reload.
        Scene active = SceneManager.GetActiveScene();
        if (active.isDirty || !string.IsNullOrEmpty(active.path)) return;
        EnsureScene();
        EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("CGVR CIE3/Open Furniture Visualizer")]
    public static void OpenScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EnsureScene();
        EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("CGVR CIE3/Rebuild Furniture Scene")]
    public static void RebuildScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        BuildScene();
    }

    [MenuItem("CGVR CIE3/Maximize Game View")]
    public static void MaximizeGameView() { SetGameViewMaximized(true); }

    [MenuItem("CGVR CIE3/Restore Game View")]
    public static void RestoreGameView() { SetGameViewMaximized(false); }

    private static void SetGameViewMaximized(bool maximized)
    {
        // Use the standard Game menu and public EditorWindow API; never recreate the scene.
        if (!EditorApplication.ExecuteMenuItem("Window/General/Game"))
        {
            Debug.LogWarning("Open the Game tab and use Shift+Space to maximize or restore it.");
            return;
        }
        EditorApplication.delayCall += () =>
        {
            EditorWindow window = EditorWindow.focusedWindow;
            if (window != null && window.GetType().Name == "GameView")
            {
                window.maximized = maximized;
                window.Focus();
                window.Repaint();
            }
            else Debug.LogWarning("Focus the docked Game tab and use Shift+Space to maximize or restore it.");
        };
    }

    private static void EnsureScene()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null) BuildScene();
    }

    private static void BuildScene()
    {
        Directory.CreateDirectory("Assets/Scenes");
        AssetDatabase.Refresh();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject root = new GameObject("CIE3 - Press Play to Build Furniture Studio");
        var demo = root.AddComponent<RetailARCaseStudyDemo>();
        demo.furnitureShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shaders/PhongFurniture.shader");
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("CIE3 scene ready. Press Play for the room, furniture and graphics controls.");
    }
}
