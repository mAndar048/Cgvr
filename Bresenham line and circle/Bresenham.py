import pygame
from pygame.locals import *
from OpenGL.GL import *

WIDTH, HEIGHT = 800, 600

def put_pixel(x, y):
    glVertex2i(x, y)

def bresenham_line(x1, y1, x2, y2):
    dx = abs(x2 - x1)
    dy = abs(y2 - y1)

    sx = 1 if x2 > x1 else -1
    sy = 1 if y2 > y1 else -1

    err = dx - dy

    glBegin(GL_POINTS)

    while True:
        put_pixel(x1, y1)

        if x1 == x2 and y1 == y2:
            break

        e2 = 2 * err

        if e2 > -dy:
            err -= dy
            x1 += sx

        if e2 < dx:
            err += dx
            y1 += sy

    glEnd()

def plot(xc, yc, x, y):
    glVertex2i(xc + x, yc + y)
    glVertex2i(xc - x, yc + y)
    glVertex2i(xc + x, yc - y)
    glVertex2i(xc - x, yc - y)
    glVertex2i(xc + y, yc + x)
    glVertex2i(xc - y, yc + x)
    glVertex2i(xc + y, yc - x)
    glVertex2i(xc - y, yc - x)

def bresenham_circle(xc, yc, r):

    x = 0
    y = r

    d = 3 - 2 * r

    glBegin(GL_POINTS)

    while x <= y:

        plot(xc, yc, x, y)

        if d < 0:
            d = d + 4 * x + 6
        else:
            d = d + 4 * (x - y) + 10
            y -= 1

        x += 1

    glEnd()

pygame.init()
pygame.display.set_mode((WIDTH, HEIGHT), DOUBLEBUF | OPENGL)

glClearColor(1, 1, 1, 1)

glMatrixMode(GL_PROJECTION)
glLoadIdentity()
glOrtho(0, WIDTH, 0, HEIGHT, -1, 1)

running = True

while running:

    for event in pygame.event.get():
        if event.type == QUIT:
            running = False

    glClear(GL_COLOR_BUFFER_BIT)

    glColor3f(1, 0, 0)
    glPointSize(1)

    bresenham_line(10, 10 , 700, 500)

    pygame.display.flip()

pygame.quit()