# Lab 10 — Unity 3D Viewing Pipeline

Port of the original Python/OpenGL experiment in ../3d_viewing_pipeline_lab10.py.

A single rotating cube has the original six colour-coded faces and edge outlines. Two cameras share a viewpoint at (4,3,6): **left orthographic**, **right perspective**, each drawing into half the Game window via Camera.rect.

## Open
1. Unity Hub → Add → Add project from disk → select this exact Unity_3D_Viewing_Pipeline folder.
2. Open with **Unity 6000.3.0f1**, Built-in Render Pipeline. No external assets or added dependencies.
3. Open Assets/Scenes/Lab10_3DViewingPipeline.unity, or CGVR Labs → Lab 10 - Open 3D Viewing Pipeline.
4. Press **Play** and select the Game tab. Models and cameras appear at runtime; a black Game view before Play is normal.
5. Maximize Game view using Shift+Space if desired.

## Controls
- Space: pause/resume animation; R: reset all values.
- Up/Down: increase/decrease angular speed (default 30° per second).
- H: show/hide information and projection settings.
- Bottom sliders: adjust orthographic vertical half-size and perspective field of view.
- Top Play/Pause and Reset buttons.

## Pipeline
Model rotation → world coordinates → shared look-at view → orthographic or perspective projection → clip/NDC → **left/right viewport mapping**. Both views render the same six-colour cube, but perspective shows foreshortening. Orthographic does not vary projected size with depth. Unity automatically depth-tests both cameras.

This is a CGVR projection demonstration, not a 3D game. The original Python, notebook and screenshots remain untouched.

**Evidence:** Unity Play-mode screenshots and compilation/runtime test still require opening in the local Editor. No unexecuted test is claimed as passed.
