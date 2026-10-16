from OpenGL.GL import *
from OpenGL.GLU import *
from OpenGL.GLUT import *

# Original triangle
triangle = [
    (2, 4),
    (6, 4),
    (4, 8)
]

# Rotation angle
angle = 90


def draw_triangle(points):
    glBegin(GL_LINE_LOOP)

    for x, y in points:
        glVertex2f(x, y)

    glEnd()


def display():
    glClear(GL_COLOR_BUFFER_BIT)

    # ---------------- ORIGINAL TRIANGLE ----------------
    glColor3f(1.0, 0.0, 0.0)

    draw_triangle(triangle)

    # ---------------- ROTATED TRIANGLE ----------------
    glPushMatrix()

    # Rotate about origin
    glRotatef(angle, 0, 0, 1)

    glColor3f(0.0, 1.0, 0.0)

    draw_triangle(triangle)

    glPopMatrix()

    glFlush()


def init():
    glClearColor(1.0, 1.0, 1.0, 1.0)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()

    gluOrtho2D(-10, 10, -10, 10)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()


glutInit()
glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)

glutInitWindowSize(700, 700)
glutInitWindowPosition(100, 100)

glutCreateWindow(b"2D Rotation About Origin")

init()

glutDisplayFunc(display)

glutMainLoop()