# Furniture Studio - Unity CGVR CIE3

**Munnaluri Rishikesh | 1RVU23CSE291 | CS4104 Computer Graphics & Virtual Reality**  
**Case Study 3: Retail and Remote Commerce | Unity 6000.3.0f1**

This folder is the **existing and working Unity source project**. It demonstrates Phong illumination, leather UV/normal mapping and logical-to-pixel viewport coordinates in an interactive 3D furniture showroom. It is not a device-tracked AR or WebXR build. There is no requirement to rebuild or regenerate the scene for the faculty presentation.

## Open the project exactly

1. Download the repository ZIP, extract it, and locate `CASE STUDY CIE3/Unity_Retail_AR_Demo`.
2. Unity Hub -> **Add -> Add project from disk** -> select **this exact Unity_Retail_AR_Demo folder** (with `Assets`, `Packages`, `ProjectSettings`).
3. Open with **Unity 6000.3.0f1**. Use the **Built-in Render Pipeline**; wait for script compilation and importing. No additional third-party packages are needed.
4. Click **CGVR CIE3 -> Open Furniture Visualizer** or open `Assets/Scenes/CIE3_Retail_Furniture_Visualizer.unity`. This is already saved; **do not select Rebuild Furniture Scene** unless it is genuinely missing.
5. Press **Play**, then focus the **Game** tab. Furniture and UI are created at runtime, so a sparse editor scene before Play is normal. For presentation use landscape **1440 x 900** (or a readable 16:9 Game view).

## Two-minute faculty demonstration

| Step | Action | Graphics explanation |
| --- | --- | --- |
| 1 | Show full room with red chair and brown leather sofa. Press **R** if zoomed. | Retail preview environment |
| 2 | Press **1**, then **2**, then **3**. Adjust **Daylight** and **Sofa specular** sliders. | Ambient, diffuse and view-dependent specular contributions of the custom Phong-style fragment shader |
| 3 | Choose **Sofa** and press **F** to focus. Toggle **T**. | UV leather color texture on/off |
| 4 | Stay in light mode **3**; toggle **B** and orbit slightly. | Normal-map microdetail changes lighting without geometry/displacement |
| 5 | Press **V**; click **Reset point**. Read the phone result **(657.5, 708.0)** for logical **(0.25, -0.30)**. | Window-to-viewport mapping from [-1,1] to phone-example pixel rectangle |
| 6 | Click **Inset viewport** and then **Fill viewport**. | Actual Unity camera `rect` and `pixelRect`, distinct from illustrative phone mapping |

**Extra controls:** Drag inside 3D view to orbit; scroll to zoom. Left/Right arrows rotate the selected furniture. `R` resets the room camera, `F` focuses the selection, `T` toggles color texture, `B` toggles normal mapping and `V` toggles the mapping panel. Make sure the **Game** window has keyboard focus.

### Three formulas and concepts

**Phong:** `I = Ia*ka + Ip*[kd*max(N·L,0) + ks*max(R·V,0)^n]`. The included `Assets/Shaders/PhongFurniture.shader` computes these components in the fragment shader using view direction, light direction, surface normal and furniture tint.

**Texture vs bump:** The `RetailARCaseStudyDemo.cs` script generates brown leather albedo and RGB normal textures at runtime. The shader samples mesh UVs for color and decodes tangent-space normals for light calculations. Mesh positions, polygon count and silhouette remain unchanged by toggling either feature.

**Window-to-viewport:** `xv = xv_min + (xw-xw_min)*(xv_max-xv_min)/(xw_max-xw_min)`; same for `y`. With source [-1,1] and sample phone viewport x=70..1010, y=120..1800, point (0.25,-0.30) yields (657.5,708.0) px. Unity's GUI converts bottom-left screen coordinates to top-left UI positioning.

## Submission evidence and screenshots

**No genuine Unity runtime screenshots are currently in GitHub `../outputs/`.** See [exact screenshot capture instructions](../outputs/README.md). A final PDF containing actual Unity output screenshots cannot truthfully be completed until images are supplied. The existing `../Previews/` images are **AI-generated concept illustrations**, not screenshots:

- [Generated room concept (not Unity output)](../Previews/generated-room-preview.png)
- [Generated material concept (not Unity output)](../Previews/generated-material-preview.png)

Read the [Unity case-study report](../UNITY_CASE_STUDY_REPORT.md) for explanations, method, scope and evidence status.

## Project files

- `Assets/Scenes/CIE3_Retail_Furniture_Visualizer.unity` - saved launch scene.
- `Assets/Scripts/RetailARCaseStudyDemo.cs` - furniture generation, controls, procedural textures and viewport calculations.
- `Assets/Shaders/PhongFurniture.shader` - ambient/diffuse/specular lighting, UV texture and normal mapping.
- `Assets/Editor/CIE3SceneBuilder.cs` - **Open Furniture Visualizer** menu; the optional rebuild path is not needed.
- `Assets/Editor/CIE3Validation.cs` - optional shader/scene/numerical mapping checks.
- `Packages/` and `ProjectSettings/` - project configuration for Unity 6 Built-in pipeline.

**Verification limitation:** This documentation was updated by reading repository source, **not** by running Unity in this environment. Do not claim an executed scene validation, screenshot capture, tested performance, or device AR integration based only on this documentation. The prior project-owner Play-mode check is not a substitute for actual submitted evidence.

**If keyboard input doesn't work:** In Unity check `Edit -> Project Settings -> Player -> Other Settings -> Active Input Handling` and use the legacy **Input Manager (Old)** configuration already recorded in the supplied project settings.
