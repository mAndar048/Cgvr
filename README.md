# Computer Graphics and Virtual Reality — Lab Programs

A consolidated collection of **Computer Graphics and Virtual Reality (CGVR)** laboratory programs with runnable source code, Jupyter notebooks, output screenshots, OpenGL implementations, shader examples, and a Unity interactive 3D scene.

All files are organized by **lab number** and use clear names such as `filename_labN`.

## Lab Index

| Lab | Experiment | Main files |
|---|---|---|
| **Lab 1** | DDA and Bresenham line drawing algorithms | `dda_line_lab1.py/.ipynb`, `bresenham_line_lab1.py/.ipynb` |
| **Lab 2** | Midpoint Circle Drawing Algorithm | `midpoint_circle_lab2.py/.ipynb` |
| **Lab 3** | 2D transformations using homogeneous coordinates | `2d_transformations_lab3.py/.ipynb` |
| **Lab 4** | Composite transformations using matrix representation | `composite_transformations_lab4.py/.ipynb` |
| **Lab 5** | Cohen–Sutherland line clipping algorithm | `cohen_sutherland_line_clipping_lab5.py/.ipynb` |
| **Lab 6** | Z-buffer hidden surface removal | `z_buffer_hidden_surface_removal_lab6.py/.ipynb` |
| **Lab 7** | Phong illumination model | `phong_illumination_lab7.py/.ipynb` |
| **Lab 8** | OpenGL vertex and fragment shaders + Unity RGB triangle demo | `vertex_fragment_shaders_lab8.py/.ipynb`, `lab8/Unity_Vertex_Fragment_Shaders/` |
| **Lab 9** | Interactive 3D scene using Unity | `unity_interactive_scene_lab9/` |
| **Lab 10** | Viewing pipeline and projections + Unity dual-camera demo | `3d_viewing_pipeline_lab10.py/.ipynb`, `lab10/Unity_3D_Viewing_Pipeline/` |

## CIE3 Case Study

| Folder | Case Study | Main files |
|---|---|---|
| **CASE STUDY CIE3** | Retail and Remote Commerce - WebXR furniture visualization | `CASE_STUDY_REPORT.md`, `case_study_3_retail_ar.py`, `case_study_3_retail_ar.ipynb` |

The CIE3 case study demonstrates:
- ambient, diffuse and specular illumination,
- texture and bump mapping without increasing polygon count,
- window-to-viewport mapping for smartphone/VR displays,
- WebXR-oriented rendering optimizations.

## Repository Structure

```text
Computer-Graphics-and-Virtual-Reality/
├── CASE STUDY CIE3/
│   ├── README.md
│   ├── CASE_STUDY_REPORT.md
│   ├── case_study_3_retail_ar.py
│   ├── case_study_3_retail_ar.ipynb
│   ├── requirements_case_study.txt
│   └── outputs/
│       └── README.md
├── lab1/
│   ├── dda_line_lab1.py
│   ├── dda_line_lab1.ipynb
│   ├── bresenham_line_lab1.py
│   ├── bresenham_line_lab1.ipynb
│   └── *_output_*_lab1.png
├── lab2/
├── lab3/
├── lab4/
├── lab5/
├── lab6/
├── lab7/
├── lab8/
├── lab9/
│   └── unity_interactive_scene_lab9/
└── lab10/
```

## Output Preview

### Lab 1 — DDA & Bresenham

![DDA Output](lab1/dda_output_1_lab1.png)

![Bresenham Output](lab1/bresenham_output_1_lab1.png)

### Lab 2 — Midpoint Circle

![Midpoint Circle Output](lab2/midpoint_circle_output_lab2.png)

### Lab 3 — 2D Transformation

![2D Transformation Output](lab3/2d_translation_output_lab3.png)

### Lab 5 — Cohen–Sutherland Clipping

![Cohen Sutherland Output](lab5/cohen_sutherland_output_lab5.png)

### Lab 6 — Z-Buffer

![Z Buffer Output](lab6/z_buffer_output_lab6.png)

### Lab 7 — Phong Illumination

![Phong Output](lab7/phong_illumination_output_lab7.png)

### Lab 8 — Vertex & Fragment Shaders

![Shader Output](lab8/vertex_fragment_shaders_output_lab8.png)

### Lab 10 — 3D Viewing Pipeline

![3D Viewing Pipeline Output](lab10/3d_viewing_pipeline_output_1_lab10.png)

## Technologies Used

- Python
- NumPy
- Matplotlib
- Pygame
- PyOpenGL
- OpenGL / GLSL
- Jupyter Notebook
- Unity / C#

## Installation

```bash
pip install -r requirements.txt
```

Typical Python dependencies are:

```text
pygame
PyOpenGL
PyOpenGL_accelerate
numpy
```

Run any Python experiment directly, for example:

```bash
python lab5/cohen_sutherland_line_clipping_lab5.py
```

For the CIE3 case study:

```bash
pip install -r "CASE STUDY CIE3/requirements_case_study.txt"
python "CASE STUDY CIE3/case_study_3_retail_ar.py"
```

or open the corresponding `.ipynb` file in Jupyter Notebook / VS Code.

## Unity projects — Labs 8 and 10

- [Lab 8: Unity vertex/fragment shaders](lab8/Unity_Vertex_Fragment_Shaders/README.md): rotating RGB triangle with shader animation, pause/reset, speed controls.
- [Lab 10: Unity viewing pipeline](lab10/Unity_3D_Viewing_Pipeline/README.md): rotating coloured cube with simultaneous orthographic/perspective cameras and projection sliders.

Both are self-contained **Unity 6000.3.0f1** projects. In Unity Hub, add their folders individually (with Assets, Packages, ProjectSettings), open the saved scene and press Play. The original Python/OpenGL code and screenshots are retained. Unity Play-mode evidence is pending local validation.

## Lab 9 — Unity

The Lab 9 folder contains the C# scripts and automatic scene builder for the interactive Unity scene. It demonstrates:

- player movement and jumping
- third-person camera follow
- obstacles and rotating objects
- collectible objects and score tracking
- directional lighting
- runtime HUD
- automatic scene generation

Unity version used: **6000.3.0f1**.

## Notes

- Output screenshots were consolidated from the individual lab repositories.
- The Python programs in this repository are cleaned consolidated versions intended to be directly readable and runnable.
- Lab 3 includes translation, rotation, scaling, reflection and shearing using homogeneous matrices.
- Lab 4 demonstrates composite transformations through matrix multiplication.
- Lab 6 contains the corrected OpenGL depth-buffer setup using `pygame.GL_DEPTH_SIZE`.
- Labs 7, 8 and 10 contain complete runnable code even though some of the original standalone notebooks were empty.
- The CIE3 case study source was locally executed and verified before being added to the repository.

## Author

**Rishikesh Munnaluri**

Computer Graphics and Virtual Reality Laboratory
