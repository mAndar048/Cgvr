using UnityEditor;
using UnityEngine;

public static class CIE3Validation
{
    [MenuItem("CGVR CIE3/Validate Scene and Mapping")]
    private static void Validate()
    {
        bool pass = true;
        Shader shader = Shader.Find("CGVR/PhongFurniture");
        if (shader == null) { Debug.LogError("CIE3 missing custom Phong shader"); pass = false; }
        Vector2 result = RetailARCaseStudyDemo.WindowToViewport(
            new Vector2(0.25f, -0.30f),
            new Rect(-1f, -1f, 2f, 2f),
            new Rect(70f, 120f, 940f, 1680f));
        if (Mathf.Abs(result.x - 657.5f) > .01f || Mathf.Abs(result.y - 708f) > .01f)
        {
            Debug.LogError("CIE3 viewport mapping failed: " + result);
            pass = false;
        }
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/CIE3_Retail_Furniture_Visualizer.unity") == null)
        {
            Debug.LogError("CIE3 scene missing. Use CGVR CIE3 > Open Furniture Visualizer.");
            pass = false;
        }
        if (pass) Debug.Log("CIE3 checks passed: shader, scene asset and 657.5 x 708 mapping.");
    }
}
