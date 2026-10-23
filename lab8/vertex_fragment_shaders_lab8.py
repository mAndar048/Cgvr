import pygame
from pygame.locals import *
import numpy as np
import ctypes
from OpenGL.GL import *
from OpenGL.GL.shaders import compileShader, compileProgram

WIDTH, HEIGHT = 900, 700

VERTEX_SHADER = '''
#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec3 aColor;
out vec3 vertexColor;
uniform float uAngle;
void main()
{
    float c = cos(uAngle);
    float s = sin(uAngle);
    mat2 rotation = mat2(c, -s, s, c);
    vec2 p = rotation * aPosition;
    gl_Position = vec4(p, 0.0, 1.0);
    vertexColor = aColor;
}
'''

FRAGMENT_SHADER = '''
#version 330 core
in vec3 vertexColor;
out vec4 FragColor;
void main()
{
    FragColor = vec4(vertexColor, 1.0);
}
'''

vertices = np.array([
     0.00,  0.65, 1.0, 0.0, 0.0,
    -0.60, -0.50, 0.0, 1.0, 0.0,
     0.60, -0.50, 0.0, 0.0, 1.0,
], dtype=np.float32)

pygame.init()
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MAJOR_VERSION, 3)
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_MINOR_VERSION, 3)
pygame.display.gl_set_attribute(pygame.GL_CONTEXT_PROFILE_MASK, pygame.GL_CONTEXT_PROFILE_CORE)
pygame.display.set_mode((WIDTH, HEIGHT), DOUBLEBUF | OPENGL)
pygame.display.set_caption('Lab 8 - Vertex and Fragment Shaders')

program = compileProgram(
    compileShader(VERTEX_SHADER, GL_VERTEX_SHADER),
    compileShader(FRAGMENT_SHADER, GL_FRAGMENT_SHADER)
)

vao = glGenVertexArrays(1)
vbo = glGenBuffers(1)
glBindVertexArray(vao)
glBindBuffer(GL_ARRAY_BUFFER, vbo)
glBufferData(GL_ARRAY_BUFFER, vertices.nbytes, vertices, GL_STATIC_DRAW)

stride = 5 * vertices.itemsize
glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(0))
glEnableVertexAttribArray(0)
glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(2 * vertices.itemsize))
glEnableVertexAttribArray(1)

glBindBuffer(GL_ARRAY_BUFFER, 0)
glBindVertexArray(0)
glClearColor(0.03, 0.04, 0.07, 1.0)

clock = pygame.time.Clock()
angle = 0.0
running = True
while running:
    dt = clock.tick(60) / 1000.0
    for event in pygame.event.get():
        if event.type == QUIT or (event.type == KEYDOWN and event.key == K_ESCAPE):
            running = False

    angle += dt
    glClear(GL_COLOR_BUFFER_BIT)
    glUseProgram(program)
    glUniform1f(glGetUniformLocation(program, 'uAngle'), angle)
    glBindVertexArray(vao)
    glDrawArrays(GL_TRIANGLES, 0, 3)
    glBindVertexArray(0)
    pygame.display.flip()

glDeleteBuffers(1, [vbo])
glDeleteVertexArrays(1, [vao])
glDeleteProgram(program)
pygame.quit()
