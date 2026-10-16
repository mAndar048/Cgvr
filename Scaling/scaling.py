from OpenGL.GL import *
from OpenGL.GLU import *
from OpenGL.GLUT import *

# --------------------------------------------------
# Triangle
# --------------------------------------------------

triangle = [
    (2, 4),
    (6, 4),
    (4, 8)
]

# Scaling factors
sx = 0.5
sy = 0.5


def draw_triangle(points):
    glBegin(GL_LINE_LOOP)

    for x, y in points:
        glVertex2f(x, y)

    glEnd()


def display():
    glClear(GL_COLOR_BUFFER_BIT)

    # ---------------------------------------------
    # Original triangle
    # ---------------------------------------------
    glColor3f(1.0, 0.0, 0.0)

    draw_triangle(triangle)

    # ---------------------------------------------
    # Scaling relative to origin
    # ---------------------------------------------
    glPushMatrix()

    glScalef(sx, sy, 1)

    glColor3f(0.0, 1.0, 0.0)

    draw_triangle(triangle)

    glPopMatrix()

    glFlush()


def init():
    glClearColor(1.0, 1.0, 1.0, 1.0)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()

    gluOrtho2D(-2, 12, -2, 12)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()


glutInit()
glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)

glutInitWindowSize(700, 700)
glutInitWindowPosition(100, 100)

glutCreateWindow(b"2D Scaling")

init()

glutDisplayFunc(display)

glutMainLoop()