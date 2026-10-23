using UnityEngine;

/// <summary>
/// Lab 8: the vertex shader rotates an RGB triangle; the fragment shader
/// displays interpolated vertex colours. Based on vertex_fragment_shaders_lab8.py.
/// </summary>
[DisallowMultipleComponent]
public sealed class Lab8VertexFragmentDemo : MonoBehaviour
{
    public Shader vertexFragmentShader;

    private Camera sceneCamera;
    private Material triangleMaterial;
    private Mesh triangleMesh;
    private float rotationRadians;
    private float speedRadians = 1f;
    private bool isPlaying = true;
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;

    private void Awake()
    {
        Shader shader = vertexFragmentShader != null ? vertexFragmentShader : Shader.Find("CGVR/Lab8/VertexFragmentRGB");
        if (shader == null || !shader.isSupported)
        {
            Debug.LogError("Lab 8 requires Assets/Shaders/Lab8VertexFragmentRGB.shader and the Built-in Render Pipeline.");
            return;
        }

        GameObject cameraObject = new GameObject("Lab 8 Camera");
        sceneCamera = cameraObject.AddComponent<Camera>();
        sceneCamera.tag = "MainCamera";
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = new Color(0.03f, 0.04f, 0.07f);
        sceneCamera.orthographic = true;
        sceneCamera.orthographicSize = 1.38f;
        sceneCamera.nearClipPlane = 0.1f;
        sceneCamera.farClipPlane = 20f;
        sceneCamera.transform.position = new Vector3(0f, 0f, -5f);
        sceneCamera.transform.rotation = Quaternion.identity;

        triangleMesh = new Mesh { name = "Three vertices: R, G, B" };
        triangleMesh.vertices = new[]
        {
            new Vector3(0f, 0.65f, 0f),
            new Vector3(-0.60f, -0.50f, 0f),
            new Vector3(0.60f, -0.50f, 0f)
        };
        triangleMesh.colors = new[] { Color.red, Color.green, Color.blue };
        triangleMesh.triangles = new[] { 0, 1, 2 };
        triangleMesh.RecalculateBounds();

        GameObject triangle = new GameObject("Rotating RGB Triangle (shader-driven)");
        triangle.AddComponent<MeshFilter>().sharedMesh = triangleMesh;
        triangleMaterial = new Material(shader) { name = "Lab 8 Vertex + Fragment Shader" };
        triangle.GetComponent<MeshRenderer>().sharedMaterial = triangleMaterial;
        ApplyAngle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isPlaying = !isPlaying;
        if (Input.GetKeyDown(KeyCode.R)) { rotationRadians = 0f; speedRadians = 1f; isPlaying = true; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { rotationRadians -= Mathf.Deg2Rad * 15f; isPlaying = false; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { rotationRadians += Mathf.Deg2Rad * 15f; isPlaying = false; }
        if (isPlaying) rotationRadians += speedRadians * Time.deltaTime;
        rotationRadians = Mathf.Repeat(rotationRadians, 2f * Mathf.PI);
        ApplyAngle();
    }

    private void ApplyAngle()
    {
        if (triangleMaterial != null) triangleMaterial.SetFloat("_Angle", rotationRadians);
    }

    private void OnGUI()
    {
        float scale = Mathf.Max(0.55f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));
        float w = Screen.width / scale;
        float h = Screen.height / scale;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 21, fontStyle = FontStyle.Bold };
            titleStyle.normal.textColor = Color.white;
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            bodyStyle.normal.textColor = new Color(.79f, .86f, .94f);
        }

        GUI.color = new Color(.06f, .11f, .18f, .95f);
        GUI.DrawTexture(new Rect(12, 12, w - 24, 126), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(28, 23, w - 56, 30), "LAB 08   /   Vertex & Fragment Shaders", titleStyle);
        GUI.Label(new Rect(28, 56, w - 56, 27), "Vertex stage: 2D rotation by uniform angle   →   Rasterizer: interpolate RGB   →   Fragment stage: output pixel colour", bodyStyle);
        if (GUI.Button(new Rect(28, 91, 120, 30), isPlaying ? "Pause (Space)" : "Play (Space)")) isPlaying = !isPlaying;
        if (GUI.Button(new Rect(157, 91, 100, 30), "Reset (R)")) { rotationRadians = 0f; speedRadians = 1f; isPlaying = true; ApplyAngle(); }
        GUI.Label(new Rect(277, 91, 160, 26), "Speed (rad/s): " + speedRadians.ToString("F2"), bodyStyle);
        speedRadians = GUI.HorizontalSlider(new Rect(442, 101, Mathf.Max(80, w - 730), 16), speedRadians, 0f, 3f);
        GUI.Label(new Rect(w - 258, 91, 224, 26), "Angle: " + (rotationRadians * Mathf.Rad2Deg).ToString("F1") + "°", bodyStyle);

        GUI.color = new Color(.06f, .11f, .18f, .93f);
        GUI.DrawTexture(new Rect(12, h - 102, w - 24, 90), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(28, h - 92, w - 55, 27), "Red  (0, +0.65)         Green (-0.60, -0.50)         Blue (+0.60, -0.50)", bodyStyle);
        GUI.Label(new Rect(28, h - 67, w - 55, 45), "The GPU shader rotates vertex positions, not the GameObject transform. Triangle colours are interpolated per pixel. Space = pause/play; ←/→ = step 15°; R = reset.", bodyStyle);
        GUI.matrix = Matrix4x4.identity;
        GUI.color = Color.white;
    }

    private void OnDestroy()
    {
        if (triangleMaterial != null) Destroy(triangleMaterial);
        if (triangleMesh != null) Destroy(triangleMesh);
    }
}
