> **Scope notice (18 October 2026):** This is the **original conceptual/Python report**, not the current Unity practical report. For faculty demonstration, see [UNITY_CASE_STUDY_REPORT.md](UNITY_CASE_STUDY_REPORT.md) and [Unity_Retail_AR_Demo](Unity_Retail_AR_Demo/README.md). Figures described below are Python-generated after running the script, **not** captured Unity Play-mode images. The Unity outputs folder currently has no genuine runtime screenshots. WebXR camera tracking and physical AR placement in the scenario/pipeline are **proposed architecture**, not implemented Unity features.

# CS4104 Computer Graphics & Virtual Reality
## CIE3 Case Study - Retail and Remote Commerce

**Student:** Munnaluri Rishikesh  
**USN:** 1RVU23CSE291  
**Selected Case Study:** Case Study 3 - Retail and Remote Commerce

---

## 1. Scenario

An interior-design startup wants to build a WebXR application that allows users to view and place furniture inside their homes using an Augmented Reality (AR) or Virtual Reality (VR) environment.

The solution must make virtual furniture look visually consistent with real-world lighting, add realistic surface detail without making the 3D mesh unnecessarily heavy, and map interaction coordinates correctly onto a smartphone or VR headset display.

---

## 2. Illumination & Color Models

### Assignment question
When a user places a virtual red velvet chair into a brightly lit, sunlit room, what illumination models (ambient, diffuse, and specular reflection) must the rendering engine compute so that the chair blends naturally with real-world lighting?

### Solution
A practical real-time renderer can use the **Phong illumination model** or a physically based equivalent. The three important components are:

1. **Ambient reflection**
   - Represents the background light present in the room.
   - Prevents the virtual object from becoming completely black on surfaces that do not directly face a light source.
   - Term: `Ia * ka`.

2. **Diffuse reflection**
   - Represents matte reflection from a surface.
   - Depends on the angle between the surface normal `N` and light direction `L`.
   - Strongest when the surface faces the light directly.
   - Term: `Ip * kd * max(N.L, 0)`.

3. **Specular reflection**
   - Produces the small bright highlights caused by reflected light.
   - Depends on the reflected light vector `R`, view direction `V`, and shininess exponent `n`.
   - Term: `Ip * ks * max(R.V, 0)^n`.

The complete simplified Phong equation is:

`I = Ia*ka + Ip[kd*max(N.L,0) + ks*max(R.V,0)^n]`

For **red velvet**, diffuse reflection should dominate because velvet is mostly matte, while the specular term should be controlled so that the fabric does not look like plastic. The base color is a dark/saturated red and should be modulated by the estimated room illumination.

In a WebXR implementation, device light estimation can be used when available so that the intensity and direction of the virtual light follow the real room. This prevents the chair from looking pasted into the camera feed.

### Output produced
Running the supplied Python program generates `outputs/illumination_models.png`, comparing:
- Ambient only
- Ambient + Diffuse
- Ambient + Diffuse + Specular

---

## 3. Texture Mapping & Bump Mapping

### Assignment question
To make a leather sofa look highly realistic without increasing polygon count, discuss the application of texture mapping and bump mapping.

### Solution

### Texture mapping
Texture mapping wraps a 2D image onto a 3D mesh using `(u,v)` texture coordinates. For a leather sofa, the texture image can store:
- leather color,
- grain,
- seams,
- small stains or imperfections,
- material variation.

This improves appearance without adding geometric detail.

### Bump mapping
Bump mapping stores small surface height variations in a texture. During lighting, the renderer perturbs the **surface normal** rather than changing the actual mesh geometry. The silhouette and polygon count stay the same, but the changing normals make pores, grain, stitching, and wrinkles react to light as if they had depth.

A normal map is a common practical form of bump mapping because RGB values directly encode the perturbed normal direction.

### Why this is important for WebXR
A high-polygon sofa would increase:
- vertex processing,
- memory usage,
- transfer size,
- rendering time,
- battery/GPU load on mobile devices.

Using a low- or medium-poly mesh with texture + bump/normal maps gives a strong visual-quality-to-performance trade-off.

### Output produced
Running the supplied program generates `outputs/texture_bump_mapping.png`, comparing:
- flat material,
- texture mapped surface,
- texture + bump mapped surface.

The geometry remains a flat rectangle in the demonstration; only the shading normals are changed for the bump-mapped version.

---

## 4. Window-to-Viewport Transformation

### Assignment question
How is window-to-viewport transformation used in the application's user interface to map the 3D interaction space accurately onto a smartphone or VR headset screen?

### Solution
After the 3D viewing pipeline projects scene coordinates into a normalized or logical interaction window, the application must convert them to actual device pixels.

If the logical window is:

`xw_min <= xw <= xw_max`

and the physical viewport is:

`xv_min <= xv <= xv_max`

then the x-coordinate is mapped using:

`xv = xv_min + (xw - xw_min) * (xv_max - xv_min)/(xw_max - xw_min)`

Similarly:

`yv = yv_min + (yw - yw_min) * (yv_max - yv_min)/(yw_max - yw_min)`

This transformation is required so that:
- selection handles appear over the correct furniture object,
- touch input maps to the correct point in the AR scene,
- UI overlays align with projected 3D positions,
- the same logical scene works across devices with different resolutions.

### Numerical example used in the implementation
Logical window:
- x: `[-1, 1]`
- y: `[-1, 1]`

Phone viewport:
- x: `[70, 1010]` pixels
- y: `[120, 1800]` pixels

A logical point `(0.25, -0.30)` maps to approximately:

**(657.5, 708.0) pixels**

### Output produced
Running the supplied program generates `outputs/viewport_mapping.png`, showing the same furniture interaction rectangle in logical AR coordinates and smartphone pixel coordinates.

---

## 5. Proposed WebXR System Pipeline

```text
Phone / VR Camera Feed
        |
        v
Environment + Light Estimation
        |
        v
Furniture 3D Mesh
   |            |
Texture Map   Bump/Normal Map
   \            /
    Material Shading
    Ambient + Diffuse + Specular
             |
             v
       3D Viewing Pipeline
             |
             v
   Projection / Screen Space
             |
             v
 Window-to-Viewport Mapping
             |
             v
 Smartphone / VR Headset Display
```

---

## 6. Optimization & Performance Analysis

Because WebXR must run interactively on phones and headsets, visual quality must be balanced with rendering cost.

Recommended optimizations:

- **Low/medium-poly furniture meshes:** use geometry only for shape and silhouette.
- **Normal/bump maps:** move fine surface details from geometry into textures.
- **Texture compression:** reduce GPU memory and download size.
- **Mipmapping:** use lower-resolution texture levels when furniture is far away.
- **Limit dynamic lights:** too many per-pixel lights increase fragment-shader cost.
- **Frustum culling:** do not render furniture or room objects outside the camera view.
- **Occlusion culling:** skip objects hidden behind other geometry where supported.
- **Level of Detail (LOD):** reduce model detail with distance.
- **Stable interaction mapping:** calculate transformations consistently for each device viewport.

These choices improve frame stability and reduce visual latency, which is important for AR/VR comfort.

---

## 7. Practical Integration

A user flow for the application is:

1. User opens the WebXR furniture visualizer.
2. Camera / VR tracking establishes the environment.
3. The user chooses a chair or sofa.
4. The app positions the 3D mesh in the tracked room.
5. Texture and bump maps add fabric/leather detail.
6. Lighting parameters are adjusted to match the environment.
7. The viewing pipeline projects the 3D model to screen space.
8. Window-to-viewport mapping aligns UI controls and touch interaction with the display.
9. The user moves, scales, or rotates the furniture interactively.

---

## 8. Result

The implemented demonstrations show that:

- Ambient, diffuse and specular components together produce a more convincing virtual material than ambient lighting alone.
- Texture and bump mapping create detailed leather appearance without increasing polygon count.
- Window-to-viewport transformation correctly converts logical interaction coordinates into device-pixel coordinates.

---

## 9. Conclusion

The case study demonstrates how core computer-graphics concepts directly solve a realistic WebXR retail problem. Illumination models make virtual furniture respond naturally to real-world light, texture and bump mapping provide efficient surface realism, and viewport transformation ensures accurate interaction on smartphones and VR displays. The same techniques can be combined in a practical AR furniture-preview system while maintaining good real-time performance.
