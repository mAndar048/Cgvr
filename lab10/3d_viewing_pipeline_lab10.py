import pygame
from pygame.locals import *
from OpenGL.GL import *
from OpenGL.GLU import *

WIDTH, HEIGHT = 1000, 600

vertices = [
    (-1,-1,-1),(1,-1,-1),(1,1,-1),(-1,1,-1),
    (-1,-1,1),(1,-1,1),(1,1,1),(-1,1,1)
]
faces = [(0,1,2,3),(4,5,6,7),(0,1,5,4),(2,3,7,6),(1,2,6,5),(0,3,7,4)]
edges = [(0,1),(1,2),(2,3),(3,0),(4,5),(5,6),(6,7),(7,4),(0,4),(1,5),(2,6),(3,7)]
colors = [(1,.2,.2),(.2,1,.3),(.2,.4,1),(1,.8,.2),(.8,.2,1),(.2,1,1)]


def draw_cube():
    glBegin(GL_QUADS)
    for i, face in enumerate(faces):
        glColor3f(*colors[i])
        for v in face:
            glVertex3fv(vertices[v])
    glEnd()

    glColor3f(0,0,0)
    glLineWidth(2)
    glBegin(GL_LINES)
    for edge in edges:
        for v in edge:
            glVertex3fv(vertices[v])
    glEnd()


def setup_viewport(x, width, perspective):
    glViewport(x, 0, width, HEIGHT)
    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    if perspective:
        gluPerspective(45, width / HEIGHT, 0.1, 100)
    else:
        glOrtho(-3, 3, -3, 3, 1, 20)
    glMatrixMode(GL_MODELVIEW)


def draw_view(angle, perspective):
    glLoadIdentity()
    gluLookAt(4, 3, 6, 0, 0, 0, 0, 1, 0)
    glRotatef(angle, 1, 1, 0)
    draw_cube()


pygame.init()
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MAJOR_VERSION, 2)
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MINOR_VERSION, 1)
pygame.display.gl_set_attribute(pygame.GL_DEPTH_SIZE, 24)
pygame.display.set_mode((WIDTH, HEIGHT), DOUBLEBUF | OPENGL)
pygame.display.set_caption('Lab 10 - 3D Viewing Pipeline')

glEnable(GL_DEPTH_TEST)
glDepthFunc(GL_LESS)
glClearColor(0.05, 0.06, 0.09, 1.0)
clock = pygame.time.Clock()
angle = 0.0
running = True

while running:
    dt = clock.tick(60) / 1000.0
    for event in pygame.event.get():
        if event.type == QUIT or (event.type == KEYDOWN and event.key == K_ESCAPE):
            running = False

    angle += 30 * dt
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)

    setup_viewport(0, WIDTH // 2, False)
    glClear(GL_DEPTH_BUFFER_BIT)
    draw_view(angle, False)

    setup_viewport(WIDTH // 2, WIDTH // 2, True)
    glClear(GL_DEPTH_BUFFER_BIT)
    draw_view(angle, True)

    pygame.display.flip()

pygame.quit()
