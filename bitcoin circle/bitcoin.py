import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *

WIDTH = 800
HEIGHT = 600

pygame.init()
pygame.display.set_mode((WIDTH, HEIGHT), DOUBLEBUF | OPENGL)

glOrtho(-20, 20, -20, 20, -1, 1)

glPointSize(5)


def plot_points(xc, yc, x, y):
    glVertex2i(xc + x, yc + y)
    glVertex2i(xc - x, yc + y)
    glVertex2i(xc + x, yc - y)
    glVertex2i(xc - x, yc - y)

    glVertex2i(xc + y, yc + x)
    glVertex2i(xc - y, yc + x)
    glVertex2i(xc + y, yc - x)
    glVertex2i(xc - y, yc - x)


def midpoint_circle(xc, yc, r):

    x = 0
    y = r

    p = 1 - r

    glBegin(GL_POINTS)

    while x <= y:

        plot_points(xc, yc, x, y)

        if p < 0:
            p = p + 2 * x + 3
        else:
            p = p + 2 * x - 2 * y + 5
            y = y - 1

        x = x + 1

    glEnd()


while True:

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            quit()

    glClear(GL_COLOR_BUFFER_BIT)

    midpoint_circle(0, 0, 8)

    pygame.display.flip()