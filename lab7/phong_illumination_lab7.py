import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *

WIDTH, HEIGHT = 900, 700
pygame.init()
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MAJOR_VERSION, 2)
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MINOR_VERSION, 1)
pygame.display.gl_set_attribute(pygame.GL_DEPTH_SIZE, 24)
pygame.display.set_mode((WIDTH, HEIGHT), DOUBLEBUF | OPENGL)
pygame.display.set_caption('Lab 7 - Phong Illumination Model')

glEnable(GL_DEPTH_TEST)
glEnable(GL_LIGHTING)
glEnable(GL_LIGHT0)
glShadeModel(GL_SMOOTH)
glClearColor(0.03, 0.04, 0.07, 1.0)

glMatrixMode(GL_PROJECTION)
glLoadIdentity()
gluPerspective(45, WIDTH / HEIGHT, 0.1, 100.0)
glMatrixMode(GL_MODELVIEW)

# Phong illumination components: ambient + diffuse + specular
glLightfv(GL_LIGHT0, GL_POSITION, [4.0, 4.0, 5.0, 1.0])
glLightfv(GL_LIGHT0, GL_AMBIENT, [0.2, 0.2, 0.2, 1.0])
glLightfv(GL_LIGHT0, GL_DIFFUSE, [1.0, 1.0, 1.0, 1.0])
glLightfv(GL_LIGHT0, GL_SPECULAR, [1.0, 1.0, 1.0, 1.0])

glMaterialfv(GL_FRONT, GL_AMBIENT, [0.15, 0.05, 0.05, 1.0])
glMaterialfv(GL_FRONT, GL_DIFFUSE, [0.8, 0.15, 0.15, 1.0])
glMaterialfv(GL_FRONT, GL_SPECULAR, [1.0, 1.0, 1.0, 1.0])
glMaterialf(GL_FRONT, GL_SHININESS, 64.0)

quadric = gluNewQuadric()
angle_x = 15.0
angle_y = 0.0
clock = pygame.time.Clock()
running = True

while running:
    dt = clock.tick(60) / 1000.0
    for event in pygame.event.get():
        if event.type == QUIT or (event.type == KEYDOWN and event.key == K_ESCAPE):
            running = False

    keys = pygame.key.get_pressed()
    if keys[K_LEFT]: angle_y -= 70 * dt
    if keys[K_RIGHT]: angle_y += 70 * dt
    if keys[K_UP]: angle_x -= 70 * dt
    if keys[K_DOWN]: angle_x += 70 * dt

    angle_y += 18 * dt

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glLoadIdentity()
    glTranslatef(0.0, 0.0, -7.0)
    glRotatef(angle_x, 1.0, 0.0, 0.0)
    glRotatef(angle_y, 0.0, 1.0, 0.0)
    gluSphere(quadric, 2.0, 64, 64)

    pygame.display.flip()

gluDeleteQuadric(quadric)
pygame.quit()
