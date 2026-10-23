# Evidence folder - actual Unity Game-view screenshots

**CS4104 | CIE3 | Case Study 3: Retail and Remote Commerce**  
**Munnaluri Rishikesh | 1RVU23CSE291**

## Current state (18 October 2026)

**Only this README is currently present. No actual Unity screenshots have been submitted to this folder.** The two images in `../Previews/` are AI-generated concepts and are explicitly **not** genuine runtime screenshots. Screenshots are required to turn the current documentation-only report into the final screenshot-inclusive faculty PDF.

## Capture genuine Play-mode evidence

1. Open `../Unity_Retail_AR_Demo` in Unity Hub using **6000.3.0f1**.
2. Menu **CGVR CIE3 -> Open Furniture Visualizer**; press **Play** and select **Game**.
3. Use a readable desktop landscape Game view (suggested 1440 x 900), keep overlays visible where needed and make sure red chair/sofa are not cropped.
4. Capture with **Win + Shift + S** (or a direct Game-view capture). Crop only surrounding desktop chrome if desired, not portions that change the evidence. Don't retouch materials/UI or remove error indicators.
5. Save as **PNG** using the exact names below in this `CASE STUDY CIE3/outputs/` directory. Verify each image opens.
6. Provide these files to update the case-study README, Unity-specific report and screenshot-inclusive PDF. The source code does not need to be rebuilt.

| File | How to configure the app | What it must visibly prove |
| --- | --- | --- |
| `unity_01_room_overview.png` | Press **R** and use full-room camera; lighting mode **3** | Both red chair and brown sofa visible together, recognizable Unity Game view |
| `unity_02_phong_ambient.png` | Mode **1**, hold the camera angle | Ambient-only lighting state |
| `unity_03_phong_full.png` | Mode **3**, same camera/brightness | Combined ambient, diffuse, specular state |
| `unity_04_sofa_texture_off.png` | Select **Sofa**, press **F**, set **T** off; keep **B** fixed | Color texture OFF |
| `unity_05_sofa_texture_on.png` | Same view, **T** on, **B** unchanged | Color texture ON |
| `unity_06_sofa_bump_off.png` | Same sofa view, light mode **3**, **B** off, **T** fixed | Normal/bump OFF |
| `unity_07_sofa_bump_on.png` | Same sofa view/light, **B** on | Normal/bump ON |
| `unity_08_viewport_mapping.png` | **V** on, click **Reset point**, full panel visible | `(0.25,-0.30)` maps to `(657.5,708.0)` in phone example |
| `unity_09_inset_viewport.png` | Press **Inset viewport**, keep room/mapping panel visible | Actual camera rectangle changed in Game view |

Keep comparison angles, zoom and sliders matching between ON/OFF screenshots. A smartphone-sized Game view is **not** required; the phone coordinates are an illustrative mathematical viewport, not device testing.

## Honest evidence policy

- A capture counts as a **genuine Unity screenshot** only if it was made in the actual running Unity project and left materially unaltered.
- Do not copy `../Previews/generated-room-preview.png` or `generated-material-preview.png` here: those are AI illustrations.
- Do not rename Python charts as Unity output. The Python generator in the parent folder can produce separate **supplementary** plots, but it does not generate Unity screenshots.
- This file deliberately contains no broken Markdown image embeds before the target images exist.
