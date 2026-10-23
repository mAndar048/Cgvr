using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class Lab10SceneMenu
{
    private const string ScenePath="Assets/Scenes/Lab10_3DViewingPipeline.unity";
    [MenuItem("CGVR Labs/Lab 10 - Open 3D Viewing Pipeline")]
    private static void OpenLab10()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode) return;
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(ScenePath);
    }
    [MenuItem("CGVR Labs/Lab 10 - Validate Scene and Shader")]
    private static void Validate()
    {
        bool ok=AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null &&
                AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shaders/Lab10FaceColors.shader")!=null;
        if(ok) Debug.Log("Lab 10: scene and shader found. Press Play to test.");
        else Debug.LogError("Lab 10: missing scene or shader.");
    }
}
