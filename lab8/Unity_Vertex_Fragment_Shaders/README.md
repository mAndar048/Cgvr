# Lab 8 — Vertex and Fragment Shaders (Unity 6.3)

Unity implementation of the existing Python/OpenGL experiment in ../vertex_fragment_shaders_lab8.py.

**What it demonstrates:** a triangle with **red, green, and blue vertices**. Its **vertex shader** rotates the triangle using a _Angle uniform (radians), and the **fragment shader** outputs smoothly interpolated vertex colours. GPU geometry and interpolation match the conceptual stages of the OpenGL example; the original Python file is preserved unchanged.

## Open and run
1. In Unity Hub, Add → Add project from disk → select this Unity_Vertex_Fragment_Shaders folder (containing Assets, Packages, ProjectSettings).
2. Use Unity Editor **6000.3.0f1**, Built-in Render Pipeline. No external packages or imported artwork.
3. Open Assets/Scenes/Lab8_VertexFragmentShaders.unity, or menu CGVR Labs → Lab 8 - Open Vertex and Fragment Shader Demo.
4. Press **Play**, then select **Game**. The triangle and camera are created in Play mode; no camera before Play is expected.
5. For screenshots, maximize the Game tab with Shift+Space and ensure both shader stages are described onscreen.

## Controls
- **Space** pause/resume rotation.
- **R** reset angle, 1 rad/s speed, resume.
- **Left/Right** step rotation by 15 degrees (pauses auto-rotation).
- GUI speed slider: 0 to 3 radians/second.
- GUI Play/Pause and Reset buttons.

## Interpretation
- Vertex shader computes (x', y') = (x cos θ − y sin θ, x sin θ + y cos θ).
- Rasterization interpolates vertex colours between the three corners.
- Fragment shader returns the interpolated RGB colour at each covered pixel.
- The triangle's **Transform is not rotated**: rotation is performed by the GPU vertex stage.

## Expected evidence (not yet captured in this repository)
- Game view with the rotating RGB triangle, visible angle and speed.
- A second view paused at a different angle.

No actual Unity screenshots or Play-mode benchmark are claimed. Use your local Unity Editor to verify actual results.
