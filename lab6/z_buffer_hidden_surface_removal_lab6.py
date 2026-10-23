import pygame
from pygame.locals import *
from OpenGL.GL import *

WIDTH, HEIGHT = 1000, 600

pygame.init()
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MAJOR_VERSION, 2)
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MINOR_VERSION, 1)
pygame.display.gl_set_attribute(pygame.GL_DEPTH_SIZE, 24)
pygame.display.set_mode((WIDTH, HEIGHT), DOUBLEBUF | OPENGL)
pygame.display.set_caption('Lab 6 - Z-Buffer Hidden Surface Removal')

glClearColor(0.05, 0.06, 0.09, 1.0)
glMatrixMode(GL_PROJECTION)
glLoadIdentity()
glOrtho(-1.3, 1.3, -1.1, 1.1, -5, 5)
glMatrixMode(GL_MODELVIEW)


def draw_scene():
    # Rear blue triangle
    glColor3f(0.15, 0.45, 1.0)
    glBegin(GL_TRIANGLES)
    glVertex3f(-0.8, -0.6, -0.4)
    glVertex3f(0.8, -0.6, -0.4)
    glVertex3f(0.0, 0.85, -0.4)
    glEnd()

    # Front red triangle, partially overlapping
    glColor3f(1.0, 0.2, 0.2)
    glBegin(GL_TRIANGLES)
    glVertex3f(-0.65, -0.4, 0.2)
    glVertex3f(0.65, -0.4, 0.2)
    glVertex3f(0.0, 0.65, 0.2)
    glEnd()


running = True
while running:
    for event in pygame.event.get():
        if event.type == QUIT or (event.type == KEYDOWN and event.key == K_ESCAPE):
            running = False

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)

    # Left viewport: depth test disabled
    glViewport(0, 0, WIDTH // 2, HEIGHT)
    glDisable(GL_DEPTH_TEST)
    glClear(GL_DEPTH_BUFFER_BIT)
    glLoadIdentity()
    draw_scene()

    # Right viewport: depth test enabled
    glViewport(WIDTH // 2, 0, WIDTH // 2, HEIGHT)
    glEnable(GL_DEPTH_TEST)
    glDepthFunc(GL_LESS)
    glClear(GL_DEPTH_BUFFER_BIT)
    glLoadIdentity()
    draw_scene()

    pygame.display.flip()

pygame.quit()
