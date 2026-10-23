using System;
using UnityEngine;

/// <summary>
/// CS4104 / CIE3 - Case Study 3, Retail and Remote Commerce.
/// Generates a model room, a velvet chair, a leather sofa, and a live visualization
/// of Phong lighting, texture/normal mapping, and window-to-viewport conversion.
/// Built-in Render Pipeline; no external assets or packages required.
/// This is a 3D showroom / AR preparation demo, NOT a live phone camera AR app.
/// </summary>
public sealed class RetailARCaseStudyDemo : MonoBehaviour
{
    [SerializeField] public Shader furnitureShader;
    private Camera sceneCamera;
    private Camera backgroundCamera;
    private Vector2 logicalPoint = new Vector2(.25f, -.30f);
    private int viewportMode;
    private bool presentationMode;
    private bool focusSelected;
    private int activePanel;
    private Vector2 controlsScroll;
    private Renderer[] chairRenderers;
    private Renderer[] sofaRenderers;
    private Renderer leftWallRenderer;

    // Keep text readable in a docked Game view instead of shrinking a 1280px canvas.
    private float GuiScale => Mathf.Clamp(Screen.height / 900f, .9f, 1.25f);
    private float GuiWidth => Screen.width / GuiScale;
    private float GuiHeight => Screen.height / GuiScale;
    private bool SidePanel => GuiWidth >= 1040f;
    private Rect HeaderRect => new Rect(12, 10, GuiWidth - 24, GuiWidth < 650 ? 86 : 62);
    private Rect FooterRect => new Rect(12, GuiHeight - 50, GuiWidth - 24, 40);
    private Rect ControlsRect
    {
        get
        {
            float top = HeaderRect.yMax + 12;
            float height = Mathf.Clamp((FooterRect.y - top - 24) * .42f, 118, 280);
            return SidePanel
                ? new Rect(12, top, 302, Mathf.Max(40, FooterRect.y - top - 12))
                : new Rect(12, FooterRect.y - height - 12, GuiWidth - 24, height);
        }
    }
    private Rect SceneRect
    {
        get
        {
            float left = !presentationMode && SidePanel ? ControlsRect.xMax + 12 : 12;
            float top = HeaderRect.yMax + 12;
            float bottom = !presentationMode && !SidePanel ? ControlsRect.y - 12 : FooterRect.y - 12;
            return new Rect(left, top, Mathf.Max(1, GuiWidth - left - 12), Mathf.Max(1, bottom - top));
        }
    }
    private Light sun;
    private Transform chairRoot;
    private Transform sofaRoot;
    private Material chairMaterial;
    private Material sofaMaterial;
    private bool showTexture = true;
    private bool showBump = true;
    private int lightingMode = 2;
    private int selectedFurniture = 0;
    private float orbitYaw = -10f;
    private float orbitPitch = 14f;
    private float zoom = 1f;
    private float roomBrightness = 1f;
    private float specularStrength = 0.32f;
    private bool isDragging;
    private Vector2 previousMouse;
    private GUIStyle headingStyle;
    private GUIStyle labelStyle;
    private GUIStyle captionStyle;
    private GUIStyle buttonStyle;
    private GUIStyle toggleStyle;
    private static readonly string[] PanelNames = { "Lighting", "Materials", "Mapping" };
    private static readonly string[] LightingNames = { "Ambient only", "Ambient + diffuse", "Ambient + diffuse + specular" };
    private Texture2D leatherAlbedo;
    private Texture2D leatherNormals;
    private Material interiorMaterial;

    private const float LogicalX = 0.25f;
    private const float LogicalY = -0.30f;
    private readonly Rect phoneViewportPixels = new Rect(70, 120, 940, 1680);

    public static Vector2 WindowToViewport(Vector2 point, Rect logicalWindow, Rect viewport)
    {
        if (logicalWindow.width <= 0f || logicalWindow.height <= 0f)
            throw new ArgumentException("Logical window must have positive width and height.");
        return new Vector2(
            viewport.xMin + (point.x - logicalWindow.xMin) * viewport.width / logicalWindow.width,
            viewport.yMin + (point.y - logicalWindow.yMin) * viewport.height / logicalWindow.height);
    }

    private void Start()
    {
        BuildMaterials();
        BuildRoom();
        BuildFurniture();
        chairRenderers = chairRoot.GetComponentsInChildren<Renderer>();
        sofaRenderers = sofaRoot.GetComponentsInChildren<Renderer>();
        BuildLightingAndCamera();
        ApplyGraphicsSettings();
    }

    private void BuildMaterials()
    {
        Shader phong = furnitureShader != null ? furnitureShader : Shader.Find("CGVR/PhongFurniture");
        if (phong == null)
        {
            Debug.LogError("Missing shader: Assets/Shaders/PhongFurniture.shader. Use the built-in render pipeline.");
            phong = Shader.Find("Standard");
        }
        if (phong == null) phong = Shader.Find("Diffuse");

        chairMaterial = new Material(phong) { name = "Red Velvet - Phong" };
        chairMaterial.SetColor("_Color", new Color(0.70f, 0.065f, 0.095f));
        chairMaterial.SetFloat("_AmbientStrength", 0.28f);
        chairMaterial.SetFloat("_DiffuseStrength", 1f);
        chairMaterial.SetFloat("_SpecularStrength", 0.14f);
        chairMaterial.SetFloat("_Shininess", 24f);

        sofaMaterial = new Material(phong) { name = "Brown Leather - Texture & Bump" };
        sofaMaterial.SetColor("_Color", new Color(0.78f, 0.58f, 0.40f));
        sofaMaterial.SetFloat("_AmbientStrength", 0.28f);
        sofaMaterial.SetFloat("_DiffuseStrength", 1f);
        sofaMaterial.SetFloat("_SpecularStrength", 0.30f);
        sofaMaterial.SetFloat("_Shininess", 38f);
        sofaMaterial.SetFloat("_BumpStrength", .75f);

        BuildLeatherTextures();
        sofaMaterial.SetTexture("_MainTex", leatherAlbedo);
        sofaMaterial.SetTexture("_BumpMap", leatherNormals);
        sofaMaterial.mainTextureScale = new Vector2(2f, 2f);
        interiorMaterial = MakeStandard(new Color(0.82f, 0.79f, 0.72f));
    }

    private static Material MakeStandard(Color color)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");
        Material material = new Material(shader) { color = color };
        if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", .18f);
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0f);
        return material;
    }

    // Deterministic procedural texture; a single rectangle mesh still gets detailed leather pores.
    private void BuildLeatherTextures()
    {
        const int size = 256;
        leatherAlbedo = new Texture2D(size, size, TextureFormat.RGBA32, true, false);
        leatherAlbedo.name = "Procedural Leather Albedo";
        leatherNormals = new Texture2D(size, size, TextureFormat.RGBA32, true, true);
        leatherNormals.name = "Procedural Leather Normal Map";
        float[,] height = new float[size, size];
        System.Random rng = new System.Random(4104);
        for (int y = 0; y < size; ++y)
        for (int x = 0; x < size; ++x)
        {
            float wx = (float)x / size;
            float wy = (float)y / size;
            height[x, y] = 0.50f + 0.075f * Mathf.Sin(wx * Mathf.PI * 28f + Mathf.Sin(wy * Mathf.PI * 12f))
                         * Mathf.Cos(wy * Mathf.PI * 32f) + 0.035f * ((float)rng.NextDouble() - 0.5f);
            float c = Mathf.Clamp01(0.76f + (height[x, y] - 0.5f) * 0.7f);
            leatherAlbedo.SetPixel(x, y, new Color(c, c * 0.87f, c * 0.72f, 1f));
        }
        for (int y = 0; y < size; ++y)
        for (int x = 0; x < size; ++x)
        {
            float dx = (height[(x + 1) % size, y] - height[(x + size - 1) % size, y]);
            float dy = (height[x, (y + 1) % size] - height[x, (y + size - 1) % size]);
            Vector3 n = new Vector3(-dx * 3f, -dy * 3f, 1f).normalized;
            // Uncompressed, linear RGB tangent-space normals; decoded explicitly in our shader.
            leatherNormals.SetPixel(x, y, new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f,
                                                     n.z * 0.5f + 0.5f, 1f));
        }
        leatherAlbedo.wrapMode = TextureWrapMode.Repeat;
        leatherNormals.wrapMode = TextureWrapMode.Repeat;
        leatherAlbedo.filterMode = FilterMode.Trilinear;
        leatherNormals.filterMode = FilterMode.Trilinear;
        leatherAlbedo.Apply();
        leatherNormals.Apply();
    }

    private static GameObject Cube(string name, Vector3 position, Vector3 size, Material material, Transform parent = null)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        if (parent != null) cube.transform.SetParent(parent, false);
        cube.transform.localPosition = position;
        cube.transform.localScale = size;
        cube.GetComponent<Renderer>().sharedMaterial = material;
        return cube;
    }

    private static GameObject Sphere(string name, Vector3 position, Vector3 size, Material material, Transform parent = null)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        if (parent != null) sphere.transform.SetParent(parent, false);
        sphere.transform.localPosition = position;
        sphere.transform.localScale = size;
        sphere.GetComponent<Renderer>().sharedMaterial = material;
        return sphere;
    }

    private void BuildRoom()
    {
        Material floor = MakeStandard(new Color(0.57f, 0.58f, 0.58f));
        Material wall = MakeStandard(new Color(0.83f, 0.82f, 0.78f));
        Material trim = MakeStandard(new Color(0.35f, 0.39f, 0.42f));
        Material rug = MakeStandard(new Color(0.19f, 0.28f, 0.35f));
        Material glass = MakeStandard(new Color(0.72f, 0.86f, 0.93f));
        Material plant = MakeStandard(new Color(0.13f, 0.38f, 0.27f));
        Cube("Floor", new Vector3(0, -0.14f, 0), new Vector3(17f, 0.24f, 13f), floor);
        Cube("Back Wall", new Vector3(0, 3.1f, 4.6f), new Vector3(17f, 6.5f, 0.18f), wall);
        leftWallRenderer = Cube("Left Wall", new Vector3(-8.35f, 3.1f, 0), new Vector3(.18f, 6.5f, 13f), wall).GetComponent<Renderer>();
        Cube("Rug", new Vector3(0, .01f, -.4f), new Vector3(8.6f, .025f, 4.6f), rug);
        Cube("Skirting Board", new Vector3(0, .10f, 4.46f), new Vector3(16.8f, .25f, .14f), trim);
        Cube("Window Glass", new Vector3(-4.9f, 3.15f, 4.43f), new Vector3(2.7f, 2.65f, .06f), glass);
        Cube("Window Vertical Trim", new Vector3(-4.9f, 3.15f, 4.33f), new Vector3(.07f, 2.75f, .1f), trim);
        Cube("Window Horizontal Trim", new Vector3(-4.9f, 3.15f, 4.33f), new Vector3(2.82f, .07f, .1f), trim);
        Cube("Plant Pot", new Vector3(6.0f, .38f, 2.7f), new Vector3(.7f, .74f, .7f), trim);
        for (int i = 0; i < 7; i++)
        {
            float a = i * 1.19f;
            Sphere("Plant Leaf", new Vector3(6f + Mathf.Cos(a) * .36f, 1.0f + i * .16f, 2.7f + Mathf.Sin(a) * .36f),
                new Vector3(.22f, .60f, .28f), plant);
        }
        Cube("Display Platform Chair", new Vector3(-2.2f, .08f, 0f), new Vector3(3.0f, .16f, 2.8f), interiorMaterial);
        Cube("Display Platform Sofa", new Vector3(1.55f, .08f, 0f), new Vector3(4.4f, .16f, 2.8f), interiorMaterial);
    }

    private void BuildFurniture()
    {
        Material wood = MakeStandard(new Color(.30f, .18f, .12f));
        chairRoot = new GameObject("Red Velvet Chair").transform;
        chairRoot.position = new Vector3(-2.2f, .16f, 0f);
        Cube("Chair Seat", new Vector3(0, 1.10f, 0), new Vector3(1.72f, .34f, 1.42f), chairMaterial, chairRoot);
        Cube("Chair Back", new Vector3(0, 1.82f, .54f), new Vector3(1.72f, 1.56f, .30f), chairMaterial, chairRoot);
        Cube("Chair Left Arm", new Vector3(-.82f, 1.40f, -.05f), new Vector3(.23f, .60f, 1.48f), chairMaterial, chairRoot);
        Cube("Chair Right Arm", new Vector3(.82f, 1.40f, -.05f), new Vector3(.23f, .60f, 1.48f), chairMaterial, chairRoot);
        for (int sx = -1; sx <= 1; sx += 2)
        for (int sz = -1; sz <= 1; sz += 2)
            Cube("Chair Leg", new Vector3(sx * .65f, .50f, sz * .5f), new Vector3(.18f, .90f, .19f), wood, chairRoot);
        Sphere("Chair Cushion", new Vector3(0, 1.30f, -.15f), new Vector3(1.5f, .16f, 1.10f), chairMaterial, chairRoot);

        sofaRoot = new GameObject("Leather Sofa").transform;
        sofaRoot.position = new Vector3(1.55f, .16f, 0f);
        Cube("Sofa Body", new Vector3(0, 0.93f, 0), new Vector3(3.58f, .65f, 1.52f), sofaMaterial, sofaRoot);
        Cube("Sofa Back", new Vector3(0, 1.60f, .57f), new Vector3(3.58f, 1.38f, .38f), sofaMaterial, sofaRoot);
        Cube("Left Arm", new Vector3(-1.67f, 1.25f, -.02f), new Vector3(.35f, .92f, 1.54f), sofaMaterial, sofaRoot);
        Cube("Right Arm", new Vector3(1.67f, 1.25f, -.02f), new Vector3(.35f, .92f, 1.54f), sofaMaterial, sofaRoot);
        for (int i = -1; i <= 1; ++i)
        {
            Cube("Leather Seat Cushion", new Vector3(i * 1.05f, 1.30f, -.13f), new Vector3(1.00f, .18f, 1.23f), sofaMaterial, sofaRoot);
            Cube("Leather Back Cushion", new Vector3(i * 1.05f, 1.78f, .34f), new Vector3(1.00f, .85f, .20f), sofaMaterial, sofaRoot);
        }
        for (int sx = -1; sx <= 1; sx += 2)
        for (int sz = -1; sz <= 1; sz += 2)
            Cube("Sofa Leg", new Vector3(sx * 1.45f, .32f, sz * .58f), new Vector3(.18f, .56f, .18f), wood, sofaRoot);
    }

    private void BuildLightingAndCamera()
    {
        // Clear pixels outside camera.rect so viewport changes never leave stale frames.
        backgroundCamera = new GameObject("Viewport Background").AddComponent<Camera>();
        backgroundCamera.depth = -10;
        backgroundCamera.cullingMask = 0;
        backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
        backgroundCamera.backgroundColor = new Color(.035f, .055f, .075f);
        sun = new GameObject("Room Daylight - Directional").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.color = new Color(1f, .91f, .82f);
        sun.intensity = roomBrightness;
        sun.transform.rotation = Quaternion.Euler(42, -32, 0);
        sun.renderMode = LightRenderMode.ForcePixel;
        sun.shadows = LightShadows.Soft;
        sun.shadowStrength = .45f;
        sun.shadowBias = .035f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.42f, .44f, .48f);
        QualitySettings.antiAliasing = 4;
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        sceneCamera = go.AddComponent<Camera>();
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = new Color(.07f, .12f, .16f);
        sceneCamera.fieldOfView = 40f;
        sceneCamera.nearClipPlane = .05f;
        sceneCamera.farClipPlane = 150f;
        sceneCamera.allowMSAA = true;
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (sceneCamera == null || Screen.width < 1 || Screen.height < 1) return;
        Rect view = SceneRect;
        if (!presentationMode && activePanel == 2 && viewportMode == 1)
            view = new Rect(view.x + view.width * .1f, view.y + view.height * .15f, view.width * .8f, view.height * .7f);
        // UI rectangles use a top-left origin; Camera.rect uses bottom-left.
        sceneCamera.rect = new Rect(view.x * GuiScale / Screen.width,
            1f - view.yMax * GuiScale / Screen.height, view.width * GuiScale / Screen.width,
            view.height * GuiScale / Screen.height);
        sceneCamera.aspect = view.width / Mathf.Max(1f, view.height);

        Bounds bounds = FurnitureBounds();
        Quaternion orbit = Quaternion.Euler(orbitPitch, orbitYaw, 0);
        Quaternion inverse = Quaternion.Inverse(orbit);
        float tanVertical = Mathf.Tan(sceneCamera.fieldOfView * .5f * Mathf.Deg2Rad);
        float tanHorizontal = tanVertical * sceneCamera.aspect;
        float fitDistance = 1f;
        // Fit all eight bounds corners at the current aspect/orbit, with a 12% margin.
        // Scroll zoom is bounded at this fit; Focus provides a larger selected-object view.
        for (int x = -1; x <= 1; x += 2)
        for (int y = -1; y <= 1; y += 2)
        for (int z = -1; z <= 1; z += 2)
        {
            Vector3 corner = inverse * Vector3.Scale(bounds.extents, new Vector3(x, y, z));
            fitDistance = Mathf.Max(fitDistance, Mathf.Abs(corner.x) * 1.12f / tanHorizontal - corner.z);
            fitDistance = Mathf.Max(fitDistance, Mathf.Abs(corner.y) * 1.12f / tanVertical - corner.z);
            fitDistance = Mathf.Max(fitDistance, sceneCamera.nearClipPlane * 2f - corner.z);
        }
        sceneCamera.transform.SetPositionAndRotation(
            bounds.center + orbit * new Vector3(0, 0, -fitDistance * zoom), orbit);
        // The room is open at the front. Hide its side wall if a wide orbit moves outside it.
        if (leftWallRenderer != null) leftWallRenderer.enabled = sceneCamera.transform.position.x > -8.15f;
    }

    private Bounds FurnitureBounds()
    {
        Bounds result = new Bounds();
        bool initialized = false;
        if (!focusSelected || selectedFurniture == 0) IncludeBounds(chairRenderers, ref result, ref initialized);
        if (!focusSelected || selectedFurniture == 1) IncludeBounds(sofaRenderers, ref result, ref initialized);
        if (!initialized) return new Bounds(new Vector3(0, 1.4f, 0), new Vector3(8, 3, 2));
        // A little breathing room below the feet and around the furniture silhouette.
        result.Expand(new Vector3(.2f, .35f, .2f));
        return result;
    }

    private static void IncludeBounds(Renderer[] renderers, ref Bounds bounds, ref bool initialized)
    {
        if (renderers == null) return;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (!initialized) { bounds = renderer.bounds; initialized = true; }
            else bounds.Encapsulate(renderer.bounds);
        }
    }

    private void ApplyGraphicsSettings()
    {
        if (chairMaterial == null || sofaMaterial == null) return;
        foreach (Material m in new[] { chairMaterial, sofaMaterial })
        {
            m.SetFloat("_AmbientStrength", .25f * roomBrightness);
            m.SetFloat("_DiffuseStrength", lightingMode >= 1 ? .95f : 0f);
            m.SetFloat("_SpecularStrength", lightingMode >= 2 ? (m == chairMaterial ? .09f : specularStrength) : 0f);
            m.SetFloat("_UseTexture", m == sofaMaterial && showTexture ? 1 : 0);
            m.SetFloat("_UseBump", m == sofaMaterial && showBump ? 1 : 0);
        }
        if (sun != null) sun.intensity = roomBrightness;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) lightingMode = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) lightingMode = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) lightingMode = 2;
        if (Input.GetKeyDown(KeyCode.T)) showTexture = !showTexture;
        if (Input.GetKeyDown(KeyCode.B)) showBump = !showBump;
        if (Input.GetKeyDown(KeyCode.V))
        {
            presentationMode = false;
            SelectPanel(activePanel == 2 ? 0 : 2);
        }
        if (Input.GetKeyDown(KeyCode.P)) TogglePresentation();
        if (Input.GetKeyDown(KeyCode.Escape)) presentationMode = false;
        if (Input.GetKeyDown(KeyCode.R)) ResetCamera();
        if (Input.GetKeyDown(KeyCode.F)) FocusSelected();
        if (Input.GetKeyDown(KeyCode.Tab)) selectedFurniture = 1 - selectedFurniture;
        if (Input.GetKey(KeyCode.LeftArrow)) TurnSelected(-48f * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow)) TurnSelected(48f * Time.deltaTime);

        Vector2 guiMouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y) / GuiScale;
        bool overScene = SceneRect.Contains(guiMouse);
        if (overScene) zoom = Mathf.Clamp(zoom - Input.mouseScrollDelta.y * .08f, 1f, 2.2f);
        if (Input.GetMouseButtonDown(0) && overScene)
        {
            isDragging = true;
            previousMouse = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) || !overScene) isDragging = false;
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 now = Input.mousePosition;
            Vector2 delta = now - previousMouse;
            orbitYaw = Mathf.Clamp(orbitYaw + delta.x * .20f, -55f, 55f);
            orbitPitch = Mathf.Clamp(orbitPitch - delta.y * .16f, 6f, 50f);
            previousMouse = now;
        }
        MoveCamera();
        ApplyGraphicsSettings();
    }

    private void OnApplicationFocus(bool hasFocus) { if (!hasFocus) isDragging = false; }

    private void ResetCamera()
    {
        focusSelected = false;
        orbitYaw = -10f;
        orbitPitch = 14f;
        zoom = 1f;
        viewportMode = 0;
    }

    private void FocusSelected()
    {
        focusSelected = true;
        zoom = 1f;
        orbitYaw = -15f;
        orbitPitch = 12f;
    }

    private void TogglePresentation()
    {
        presentationMode = !presentationMode;
        isDragging = false;
        ResetCamera();
    }

    private void SelectPanel(int panel)
    {
        activePanel = panel;
        controlsScroll = Vector2.zero;
        isDragging = false;
    }

    private void TurnSelected(float degrees)
    {
        Transform t = selectedFurniture == 0 ? chairRoot : sofaRoot;
        if (t != null) t.Rotate(Vector3.up, degrees, Space.World);
    }

    private void OnGUI()
    {
        EnsureStyles();
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        Color previousBackground = GUI.backgroundColor;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * GuiScale);
        try
        {
            DrawHeader();
            DrawFooter();
            if (!presentationMode) DrawControls();
            if (!presentationMode && activePanel == 2) DrawMappedScreenPoint();
        }
        finally
        {
            GUI.color = previousColor;
            GUI.backgroundColor = previousBackground;
            GUI.matrix = previousMatrix;
        }
    }

    private void EnsureStyles()
    {
        if (headingStyle != null) return;
        headingStyle = new GUIStyle(GUI.skin.label) { fontSize = 19, fontStyle = FontStyle.Bold, wordWrap = true };
        headingStyle.normal.textColor = new Color(.94f, .96f, .97f);
        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
        labelStyle.normal.textColor = new Color(.9f, .93f, .95f);
        captionStyle = new GUIStyle(labelStyle) { fontSize = 13 };
        captionStyle.normal.textColor = new Color(.65f, .73f, .79f);
        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14, wordWrap = true, fixedHeight = 34 };
        toggleStyle = new GUIStyle(GUI.skin.toggle) { fontSize = 15, wordWrap = true };
        toggleStyle.normal.textColor = toggleStyle.onNormal.textColor = Color.white;
        toggleStyle.hover.textColor = toggleStyle.onHover.textColor = Color.white;
    }

    private static void Panel(Rect rect)
    {
        GUI.color = new Color(.075f, .11f, .145f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = new Color(.23f, .53f, .56f);
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawHeader()
    {
        Rect rect = HeaderRect;
        Panel(rect);
        bool narrow = GuiWidth < 650;
        GUI.Label(new Rect(rect.x + 14, rect.y + 6, rect.width - (narrow ? 28 : 210), 28), "CIE3 | Furniture Studio", headingStyle);
        if (!narrow) GUI.Label(new Rect(rect.x + 14, rect.y + 34, rect.width - 215, 22),
            "Retail & Remote Commerce  /  " + LightingNames[lightingMode], captionStyle);
        Rect button = narrow ? new Rect(rect.x + 14, rect.y + 43, rect.width - 28, 32)
                             : new Rect(rect.xMax - 196, rect.y + 14, 182, 34);
        if (GUI.Button(button, presentationMode ? "Show controls (P)" : "Present (P)", buttonStyle))
        {
            TogglePresentation();
            GUIUtility.ExitGUI(); // Rebuild GUILayout safely on the next layout event.
        }
    }

    private void DrawFooter()
    {
        Rect rect = FooterRect;
        Panel(rect);
        float buttonWidth = Mathf.Min(108f, (rect.width - 26f) / 4f);
        for (int i = 0; i < 4; i++)
        {
            string text = i == 0 ? "Chair" : i == 1 ? "Sofa" : i == 2 ? "Focus (F)" : "Room (R)";
            Color oldBackground = GUI.backgroundColor;
            if (i < 2 && selectedFurniture == i) GUI.backgroundColor = new Color(.4f, .8f, .8f);
            if (GUI.Button(new Rect(rect.x + 6 + i * (buttonWidth + 4), rect.y + 3, buttonWidth, 34), text, buttonStyle))
            {
                if (i < 2) selectedFurniture = i;
                else if (i == 2) FocusSelected();
                else ResetCamera();
            }
            GUI.backgroundColor = oldBackground;
        }
        float hintsX = rect.x + 12 + 4 * (buttonWidth + 4);
        if (rect.xMax - hintsX > 370)
            GUI.Label(new Rect(hintsX, rect.y + 9, rect.xMax - hintsX - 10, 24),
                "Drag orbit  |  Scroll zoom  |  Arrows rotate  |  1/2/3 light", captionStyle);
    }

    private void DrawControls()
    {
        Rect panel = ControlsRect;
        Panel(panel);
        float tabWidth = (panel.width - 24) / 3;
        for (int i = 0; i < PanelNames.Length; i++)
        {
            Color oldBackground = GUI.backgroundColor;
            if (i == activePanel) GUI.backgroundColor = new Color(.4f, .8f, .8f);
            if (GUI.Button(new Rect(panel.x + 10 + i * (tabWidth + 2), panel.y + 10, tabWidth, 34), PanelNames[i], buttonStyle) && i != activePanel)
            {
                SelectPanel(i);
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = oldBackground;
        }
        GUILayout.BeginArea(new Rect(panel.x + 12, panel.y + 54, panel.width - 24, Mathf.Max(1, panel.height - 66)));
        controlsScroll = GUILayout.BeginScrollView(controlsScroll, false, false);
        if (activePanel == 0) DrawLightingControls();
        else if (activePanel == 1) DrawMaterialControls();
        else DrawViewportOverlay();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawLightingControls()
    {
        GUILayout.Label("01 / Phong illumination", headingStyle);
        GUILayout.Label("Add each lighting term to reveal shape and highlights.", captionStyle);
        GUILayout.Space(10);
        for (int i = 0; i < LightingNames.Length; i++)
        {
            Color oldBackground = GUI.backgroundColor;
            if (lightingMode == i) GUI.backgroundColor = new Color(.4f, .8f, .8f);
            if (GUILayout.Button((i + 1) + "  " + LightingNames[i], buttonStyle)) lightingMode = i;
            GUI.backgroundColor = oldBackground;
        }
        GUILayout.Space(12);
        roomBrightness = Slider("Daylight", roomBrightness, .25f, 1.7f);
        specularStrength = Slider("Sofa specular", specularStrength, 0f, .9f);
        GUILayout.Space(8);
        GUILayout.Label("I = ambient + diffuse + specular", labelStyle);
        GUILayout.Label("Select the sofa, then Focus. Orbit to see the highlight move with the viewing direction.", captionStyle);
    }

    private void DrawMaterialControls()
    {
        GUILayout.Label("02 / Texture & bump", headingStyle);
        GUILayout.Label("Compare color detail and surface shading independently on the same sofa geometry.", captionStyle);
        GUILayout.Space(12);
        showTexture = GUILayout.Toggle(showTexture, "Texture mapping (T)", toggleStyle, GUILayout.MinHeight(34));
        showBump = GUILayout.Toggle(showBump, "Bump / normal mapping (B)", toggleStyle, GUILayout.MinHeight(34));
        GUILayout.Space(12);
        if (GUILayout.Button("Focus leather sofa", buttonStyle)) { selectedFurniture = 1; FocusSelected(); }
        GUILayout.Label("Use lighting mode 2 or 3 to see bump shading. The mesh and silhouette do not change.", captionStyle);
        GUILayout.Space(8);
        GUILayout.Label("Texture: " + (showTexture ? "ON" : "OFF") + "  |  Bump: " + (showBump ? "ON" : "OFF"), labelStyle);
    }

    private float Slider(string title, float value, float minimum, float maximum)
    {
        GUILayout.Label(title + ": " + value.ToString("F2"), labelStyle);
        value = GUILayout.HorizontalSlider(value, minimum, maximum, GUILayout.Height(20));
        GUILayout.Space(6);
        return value;
    }

    private void DrawViewportOverlay()
    {
        GUILayout.Label("03 / Window to viewport", headingStyle);
        GUILayout.Label("Logical window [-1,1] on both axes. Move the point; compare phone and Game-view coordinates.", captionStyle);
        Rect preview = GUILayoutUtility.GetRect(1, 154, GUILayout.ExpandWidth(true));
        Rect phone = new Rect(preview.x + 6, preview.y + 8, 72, 138);
        GUI.color = new Color(.16f, .28f, .36f);
        GUI.DrawTexture(phone, Texture2D.whiteTexture);
        GUI.color = Color.white;
        Rect logicalWindow = new Rect(-1, -1, 2, 2);
        Vector2 pixel = WindowToViewport(logicalPoint, logicalWindow, phoneViewportPixels);
        Vector2 point = WindowToViewport(logicalPoint, logicalWindow, new Rect(0, 0, phone.width, phone.height));
        GUI.color = new Color(1f, .6f, .3f);
        GUI.DrawTexture(new Rect(phone.x + point.x - 4, phone.yMax - point.y - 4, 8, 8), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(preview.x + 92, preview.y + 8, Mathf.Max(50, preview.width - 98), 138),
            "Phone viewport (px)\nX: 70..1010\nY: 120..1800\n\nBottom-left origin", captionStyle);
        GUILayout.Label("Phone: (" + pixel.x.ToString("F1") + ", " + pixel.y.ToString("F1") + ") px", labelStyle);
        logicalPoint.x = Slider("Logical X", logicalPoint.x, -1f, 1f);
        logicalPoint.y = Slider("Logical Y", logicalPoint.y, -1f, 1f);
        if (GUILayout.Button("Reset point (0.25, -0.30)", buttonStyle)) logicalPoint = new Vector2(LogicalX, LogicalY);
        if (GUILayout.Button(viewportMode == 0 ? "Inset camera viewport" : "Fill camera viewport", buttonStyle)) viewportMode = 1 - viewportMode;
        if (sceneCamera != null)
        {
            Rect actual = sceneCamera.pixelRect;
            Vector2 mapped = WindowToViewport(logicalPoint, logicalWindow, actual);
            GUILayout.Label("Camera: " + actual.width.ToString("F0") + " x " + actual.height.ToString("F0") + " px", labelStyle);
            GUILayout.Label("Screen point: (" + mapped.x.ToString("F1") + ", " + mapped.y.ToString("F1") + ")", labelStyle);
        }
        GUILayout.Label("Orange marker = mapped screen point. GUI drawing flips Y; the mapping formula is unchanged.", captionStyle);
    }

    private void DrawMappedScreenPoint()
    {
        if (sceneCamera == null) return;
        Rect actual = sceneCamera.pixelRect;
        Vector2 point = WindowToViewport(logicalPoint, new Rect(-1, -1, 2, 2), actual);
        GUI.color = new Color(1f, .6f, .3f);
        GUI.DrawTexture(new Rect(point.x / GuiScale - 4, (Screen.height - point.y) / GuiScale - 4, 8, 8), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void OnDestroy()
    {
        if (chairMaterial != null) Destroy(chairMaterial);
        if (sofaMaterial != null) Destroy(sofaMaterial);
        if (leatherAlbedo != null) Destroy(leatherAlbedo);
        if (leatherNormals != null) Destroy(leatherNormals);
    }
}
