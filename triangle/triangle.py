from OpenGL.GL import *
from OpenGL.GLU import *
from pygame.locals import *
import pygame

pygame.init()

display = (800, 600)
pygame.display.set_mode(display, DOUBLEBUF | OPENGL)

gluPerspective(45, display[0] / display[1], 0.1, 50.0)
glTranslatef(0, 0, -5)

angle = 0

while True:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            quit()

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)

    glPushMatrix()
    glRotatef(angle, 0, 1, 0)

    glBegin(GL_TRIANGLES)
    glColor3f(1, 0, 0)
    glVertex3f(-1, -1, 0)

    glColor3f(0, 1, 0)
    glVertex3f(1, -1, 0)

    glColor3f(0, 0, 1)
    glVertex3f(0, 1, 0)
    glEnd()

    glPopMatrix()

    angle += 1

    pygame.display.flip()
    pygame.time.wait(10)