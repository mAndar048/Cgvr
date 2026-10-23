"""CGVR CIE-3 Case Study 3: Retail and Remote Commerce.

Demonstrates three concepts requested in the assignment:
1. Ambient + diffuse + specular (Phong) illumination on a red velvet-like surface.
2. Texture mapping vs bump mapping without increasing polygon count.
3. Window-to-viewport transformation for mapping interaction coordinates to a screen.

Outputs are written to the local `outputs/` directory.
"""
from __future__ import annotations

from pathlib import Path
import numpy as np
import matplotlib.pyplot as plt

OUTPUT_DIR = Path(__file__).resolve().parent / "outputs"
OUTPUT_DIR.mkdir(parents=True, exist_ok=True)


def normalize(v: np.ndarray) -> np.ndarray:
    n = np.linalg.norm(v, axis=-1, keepdims=True)
    return v / np.clip(n, 1e-9, None)


def render_phong_demo() -> None:
    """Render a synthetic red velvet-like patch under different lighting terms."""
    size = 420
    x = np.linspace(-1.0, 1.0, size)
    y = np.linspace(-1.0, 1.0, size)
    xx, yy = np.meshgrid(x, y)
    rr2 = xx**2 + yy**2
    mask = rr2 <= 1.0

    zz = np.zeros_like(xx)
    zz[mask] = np.sqrt(1.0 - rr2[mask])
    normals = np.dstack((xx, yy, zz))
    normals = normalize(normals)

    light_dir = normalize(np.array([0.55, -0.35, 0.76], dtype=float))
    view_dir = np.array([0.0, 0.0, 1.0])

    ndotl = np.clip(np.sum(normals * light_dir, axis=-1), 0.0, 1.0)
    reflect = 2.0 * ndotl[..., None] * normals - light_dir
    reflect = normalize(reflect)
    rdotv = np.clip(np.sum(reflect * view_dir, axis=-1), 0.0, 1.0)

    ambient = 0.24
    diffuse = 0.72 * ndotl
    specular = 0.58 * (rdotv**34)

    # Soft procedural velvet texture: low-amplitude weave/noise in color only.
    weave = 0.035 * (np.sin(70 * xx) * np.sin(74 * yy))
    red_base = np.array([0.68, 0.035, 0.045])

    def make_image(a: float, d: np.ndarray, s: np.ndarray) -> np.ndarray:
        intensity = a + d
        img = red_base[None, None, :] * intensity[..., None]
        img += s[..., None] * np.array([1.0, 0.82, 0.72])[None, None, :]
        img += weave[..., None]
        img = np.clip(img, 0.0, 1.0)
        img[~mask] = np.array([0.95, 0.95, 0.95])
        return img

    ambient_img = make_image(ambient, np.zeros_like(diffuse), np.zeros_like(specular))
    diffuse_img = make_image(ambient, diffuse, np.zeros_like(specular))
    full_img = make_image(ambient, diffuse, specular)

    fig, axes = plt.subplots(1, 3, figsize=(12, 4.25))
    for ax, img, title in zip(
        axes,
        [ambient_img, diffuse_img, full_img],
        ["Ambient only", "Ambient + Diffuse", "Ambient + Diffuse + Specular"],
    ):
        ax.imshow(img)
        ax.set_title(title, fontsize=11)
        ax.axis("off")
    fig.suptitle("Illumination for a Virtual Red Velvet Chair (Phong Components)", fontsize=14)
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / "illumination_models.png", dpi=180, bbox_inches="tight")
    plt.close(fig)


def render_texture_bump_demo() -> None:
    """Show how texture and bump mapping add detail without extra mesh geometry."""
    h, w = 360, 520
    x = np.linspace(0, 1, w)
    y = np.linspace(0, 1, h)
    xx, yy = np.meshgrid(x, y)

    # Synthetic leather albedo: subtle mottling + seams.
    grain = (
        0.55 * np.sin(95 * xx + 7 * np.sin(12 * yy))
        + 0.35 * np.sin(128 * yy)
        + 0.25 * np.sin(170 * (xx + yy))
    )
    grain = grain / np.max(np.abs(grain))
    seam = np.exp(-((yy - 0.5) ** 2) / 0.0007)
    albedo_scalar = np.clip(0.56 + 0.075 * grain - 0.18 * seam, 0, 1)
    base = np.dstack([
        0.30 + 0.22 * albedo_scalar,
        0.13 + 0.10 * albedo_scalar,
        0.055 + 0.04 * albedo_scalar,
    ])

    # Height field used only to perturb normals: the mesh stays a flat rectangle.
    height = 0.58 + 0.12 * grain - 0.10 * seam
    gy, gx = np.gradient(height)
    nx = -2.2 * gx
    ny = -2.2 * gy
    nz = np.ones_like(nx)
    normals = normalize(np.dstack([nx, ny, nz]))
    light = normalize(np.array([-0.42, -0.34, 0.84]))
    diffuse = np.clip(np.sum(normals * light, axis=-1), 0.10, 1.0)
    bumped = np.clip(base * (0.43 + 0.82 * diffuse[..., None]), 0, 1)

    fig, axes = plt.subplots(1, 3, figsize=(12, 4.5))
    axes[0].imshow(np.ones_like(base) * np.array([0.45, 0.22, 0.08]))
    axes[0].set_title("Low-poly flat material")
    axes[1].imshow(base)
    axes[1].set_title("Texture mapping (color detail)")
    axes[2].imshow(bumped)
    axes[2].set_title("Texture + bump mapping (light detail)")
    for ax in axes:
        ax.axis("off")
    fig.suptitle("Leather Sofa Detail Without Increasing Polygon Count", fontsize=14)
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / "texture_bump_mapping.png", dpi=180, bbox_inches="tight")
    plt.close(fig)


def window_to_viewport(xw, yw, window, viewport):
    """Map world/window coordinates into screen/viewport coordinates."""
    xw_min, xw_max, yw_min, yw_max = window
    xv_min, xv_max, yv_min, yv_max = viewport
    xv = xv_min + (xw - xw_min) * (xv_max - xv_min) / (xw_max - xw_min)
    yv = yv_min + (yw - yw_min) * (yv_max - yv_min) / (yw_max - yw_min)
    return xv, yv


def render_viewport_demo() -> None:
    """Visualize window-to-viewport mapping used by phone/VR display interfaces."""
    chair = np.array([
        [-0.72, -0.42],
        [0.65, -0.42],
        [0.65, 0.45],
        [-0.72, 0.45],
        [-0.72, -0.42],
    ])
    window = (-1.0, 1.0, -1.0, 1.0)
    viewport = (70, 1010, 120, 1800)
    mapped = np.array([window_to_viewport(x, y, window, viewport) for x, y in chair])

    fig, axes = plt.subplots(1, 2, figsize=(10.5, 5.3))

    axes[0].plot(chair[:, 0], chair[:, 1], marker="o")
    axes[0].set_xlim(window[0], window[1])
    axes[0].set_ylim(window[2], window[3])
    axes[0].set_aspect("equal")
    axes[0].grid(True, alpha=0.35)
    axes[0].set_title("AR interaction window\n(normalized logical coordinates)")
    axes[0].set_xlabel("xw")
    axes[0].set_ylabel("yw")

    axes[1].plot(mapped[:, 0], mapped[:, 1], marker="o")
    axes[1].set_xlim(0, 1080)
    axes[1].set_ylim(0, 1920)
    axes[1].set_aspect("equal")
    axes[1].invert_yaxis()
    axes[1].grid(True, alpha=0.35)
    axes[1].set_title("Smartphone viewport\n(pixel coordinates)")
    axes[1].set_xlabel("xv (px)")
    axes[1].set_ylabel("yv (px)")

    fig.suptitle("Window-to-Viewport Transformation for an AR Furniture Interface", fontsize=14)
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / "viewport_mapping.png", dpi=180, bbox_inches="tight")
    plt.close(fig)

    example_world = (0.25, -0.30)
    example_view = window_to_viewport(*example_world, window, viewport)
    (OUTPUT_DIR / "viewport_example.txt").write_text(
        "Window = [-1,1] x [-1,1]\n"
        "Viewport = x:[70,1010], y:[120,1800]\n"
        f"Example point {example_world} maps to ({example_view[0]:.1f}, {example_view[1]:.1f}) pixels.\n",
        encoding="utf-8",
    )


def main() -> None:
    render_phong_demo()
    render_texture_bump_demo()
    render_viewport_demo()
    print(f"Generated outputs in: {OUTPUT_DIR}")


if __name__ == "__main__":
    main()
