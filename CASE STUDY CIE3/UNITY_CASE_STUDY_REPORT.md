# CS4104 - Computer Graphics & Virtual Reality
## CIE3 Case Study 3: Retail and Remote Commerce
### Unity Furniture Visualizer - Submission Report

**Student:** Munnaluri Rishikesh  
**USN:** 1RVU23CSE291  
**University:** RV University  
**Development environment:** Unity 6000.3.0f1, C#, ShaderLab/HLSL, Built-in Render Pipeline  
**Main project:** `Unity_Retail_AR_Demo`

## 1. Aim and problem statement

The faculty scenario considers a retail/home-interior WebXR system where a customer previews a red velvet chair and a leather sofa. Three related graphics problems must be explained: making furniture react to room lighting, presenting detailed leather without expensive mesh geometry, and translating logical interaction coordinates to screen pixels. The implemented assignment is a **desktop Unity 3D educational simulation of these rendering concepts**; it does **not** provide live camera tracking, spatial anchors, or a deployed WebXR experience.

## 2. Existing Unity implementation

The repository contains the saved scene `Assets/Scenes/CIE3_Retail_Furniture_Visualizer.unity`, runtime controller `Assets/Scripts/RetailARCaseStudyDemo.cs`, custom shader `Assets/Shaders/PhongFurniture.shader` and editor launch menu in `Assets/Editor/CIE3SceneBuilder.cs`. During Play mode the controller creates the demo room, basic chair/sofa meshes, procedural leather textures, lights, camera, GUI, and mathematical mapping overlay. The supplied implementation is preserved; no rebuilding is required to demonstrate it.

The red chair gives an example of predominantly diffuse, cloth-like light response. The leather sofa illustrates colored grain and fine normal-map shading on coarse meshes. The room is a simulated space lit by Unity lights; no real-world illumination measurements are imported.

## 3. Question 1 - Phong illumination and color models

**Ambient component** approximates uniform indirect/background light, keeping shadowed material visible. `Ia * ka` is the familiar simplified scalar expression. **Diffuse component** is proportional to `max(dot(N,L),0)`, where `N` is the unit surface normal and `L` is the unit direction toward the light: front-facing areas are bright, oblique areas are dim. **Specular component** estimates highlights using the reflection vector `R`, view vector `V` and shininess exponent `n`. The highlight changes as the camera or light moves.

```text
I = Ia * ka + Ip * [kd * max(N·L, 0) + ks * max(R·V, 0)^n]
```

The fragment shader separately computes ambient, diffuse and specular RGB terms, multiplies ambient/diffuse by the furniture albedo, and includes the current light color. This is an educational Phong-style forward shader rather than full physically based rendering (PBR). A dark/saturated red with less specular reflection helps the chair resemble velvet, while controlled highlights help communicate leather.

**In Unity:** Press `1` for ambient, `2` to add diffuse and `3` to add specular. Adjust daylight and sofa-specular sliders; orbit the camera to see how material appearance changes. Screen observations must be captured from the actual Unity Game tab; they have not been independently verified in this report-generation environment.

## 4. Question 2 - Texture mapping and bump mapping

**Texture mapping** uses the mesh's UV coordinates `(u,v)` to sample a two-dimensional leather albedo texture. It adds grain and color variation while preserving the sofa's geometry. **Bump mapping** changes the normal used by the lighting equation at each fragment to simulate tiny pores, scratches and surface irregularities. A **normal map** is a practical representation of bump detail: decoded texture RGB is transformed from tangent space into world space and incorporated into the shader's diffuse/specular calculations.

The code procedurally creates an albedo texture and a normal texture at runtime. The shader independently switches color texture sampling and tangent-space normal perturbation via `_UseTexture` and `_UseBump`, with `_BumpStrength` controlling effect magnitude. Fine relief modifies shading rather than triangle positions; **mesh silhouette and polygon count do not change**. The effect depends on view, lighting, texture scale, and available tangents; it is not geometric displacement.

**In Unity:** Select Sofa and press `F` to focus. Press `T` to disable/enable color mapping. Use lighting mode `3`, then press `B` to compare bump off/on; orbit to inspect the highlight difference. Capture matched pairs from the real Game tab.

## 5. Question 3 - Window-to-viewport transformation

After the 3D model/view/projection steps, a logical 2D window must be mapped to the target screen viewport. For a source rectangle `[xw_min,xw_max] x [yw_min,yw_max]` and destination pixel rectangle `[xv_min,xv_max] x [yv_min,yv_max]`:

```text
xv = xv_min + (xw - xw_min) * (xv_max - xv_min) / (xw_max - xw_min)
yv = yv_min + (yw - yw_min) * (yv_max - yv_min) / (yw_max - yw_min)
```

**Worked example:** logical window `[-1,1] x [-1,1]`; illustrative phone viewport `x=[70,1010]`, `y=[120,1800]`; point `(0.25,-0.30)`.

```text
xv = 70  + (0.25 + 1) * (1010 - 70) / 2 = 657.5 px
yv = 120 + (-0.30 + 1) * (1800 - 120) / 2 = 708.0 px
```

These coordinates use a **bottom-left** origin. The Unity immediate-mode GUI uses a **top-left** origin and reverses Y when drawing its indicator. The illustrative phone rectangle is **not the physical screen resolution**; the separate Inset/Fill control modifies `Camera.rect`, with `Camera.pixelRect` used to map a marker within the actual Game-view camera viewport.

**In Unity:** Press `V`, move Logical X and Y sliders, click **Reset point** and explain the displayed `657.5, 708.0` result. Click **Inset viewport**, observing the rendered camera rectangle change. This demonstrates coordinate conversion, not phone touch or VR tracking.

## 6. Practical procedure for faculty demonstration

1. Open Unity Hub and add folder `CASE STUDY CIE3/Unity_Retail_AR_Demo`. Use Unity `6000.3.0f1` and allow import.
2. Choose menu **CGVR CIE3 -> Open Furniture Visualizer**, or double-click `Assets/Scenes/CIE3_Retail_Furniture_Visualizer.unity`. Do not use **Rebuild** on the working project.
3. Press **Play**, switch to **Game**, choose a landscape size, and ensure both furniture items are visible.
4. Demonstrate the Phong progression by pressing **1, 2, 3** and optionally adjusting daylight and sofa specular.
5. Select Sofa and press **F**; independently compare texture **T** and bump **B** in mode **3**. Press **R** to return to the room.
6. Press **V** and **Reset point**; explain `(0.25,-0.30) -> (657.5,708.0)`. Select **Inset viewport** to demonstrate the camera screen rectangle.
7. Optionally run **CGVR CIE3 -> Validate Scene and Mapping** in the editor and inspect the Console output. The documented utility checks the shader, scene asset, and expected mapping; this report does not claim it was executed during document preparation.
8. Stop Play when finished. Save actual Play-mode screenshots externally in the parent `outputs/` folder; do not replace them with illustrations.

## 7. Screenshot evidence - REQUIRED FOR FINAL FACULTY SUBMISSION

At the time of final documentation review, the repository's `outputs/` directory has **no authentic Unity screenshots**. The nine specific requested captures and instructions are provided in [outputs/README.md](outputs/README.md). Do not submit AI concept previews as Unity execution proof. Evidence will be embedded into this report and the final screenshot-inclusive PDF **only after actual files are supplied**.

| Evidence | Suggested file name | Status |
| --- | --- | --- |
| Full furniture showroom | `unity_01_room_overview.png` | **Pending capture** |
| Phong ambient mode | `unity_02_phong_ambient.png` | **Pending capture** |
| Phong full mode | `unity_03_phong_full.png` | **Pending capture** |
| Texture off/on comparison | `unity_04_sofa_texture_off.png`, `unity_05_sofa_texture_on.png` | **Pending capture** |
| Bump off/on comparison | `unity_06_sofa_bump_off.png`, `unity_07_sofa_bump_on.png` | **Pending capture** |
| Logical phone mapping | `unity_08_viewport_mapping.png` | **Pending capture** |
| Actual inset camera view | `unity_09_inset_viewport.png` | **Pending capture** |

## 8. Expected observations, scope and limitations

Source-code inspection supports the intent to show lighting components, independent texture/bump effects and the specified mathematical mapping. The numerical example evaluates algebraically to **(657.5, 708.0) px**. Previous project-owner feedback states that the Unity scene has displayed both furniture items, but **no screenshot or fresh runtime test has been supplied to substantiate the figures**. Therefore this report does **not** assert measured image quality, FPS, supported device compatibility, editor console results or physical AR capabilities. The Photoshop-like generated images in `Previews/` are explicitly **concept illustrations, not Unity output**.

The rendering approach is a small built-in-pipeline demonstration with procedural geometry and materials. Production retail WebXR would require additional device tracking, environment light estimation, mobile UX, surface placement, occlusion and performance evaluation; none of those systems is claimed as completed here.

## 9. Conclusion

The Unity project provides an explainable teaching demonstration for Case Study 3. A custom Phong-style shader separates ambient, diffuse and specular lighting; UV color and normal mapping model sofa appearance without added mesh detail; and window-to-viewport equations map logical coordinates to an illustrative phone screen and actual camera display rectangle. **Screenshot-based visual results remain pending until genuine Unity captures are submitted.**

## Code and evidence references

- `Unity_Retail_AR_Demo/Assets/Shaders/PhongFurniture.shader`
- `Unity_Retail_AR_Demo/Assets/Scripts/RetailARCaseStudyDemo.cs`
- `Unity_Retail_AR_Demo/Assets/Editor/CIE3SceneBuilder.cs`
- `Unity_Retail_AR_Demo/Assets/Editor/CIE3Validation.cs`
- `outputs/README.md` (capture protocol and status)
- `Previews/README.md` (AI-generated images, not runtime evidence)
