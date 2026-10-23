import math
import pygame
import numpy as np

WIDTH, HEIGHT = 1050, 650
BG = (14,17,27)
TEXT = (238,242,250)
ORIGINAL = (225,230,240)
RESULT = (70,220,255)


def T(tx, ty):
    return np.array([[1,0,tx],[0,1,ty],[0,0,1]], float)


def R(deg):
    a = math.radians(deg)
    c, s = math.cos(a), math.sin(a)
    return np.array([[c,-s,0],[s,c,0],[0,0,1]], float)


def S(sx, sy):
    return np.array([[sx,0,0],[0,sy,0],[0,0,1]], float)


def transform(points, matrix):
    hom = np.column_stack((points, np.ones(len(points))))
    return (matrix @ hom.T).T[:, :2]


def to_screen(points):
    ox, oy = WIDTH//2, HEIGHT//2 + 30
    return [(int(ox+x), int(oy-y)) for x,y in points]


def main():
    pygame.init()
    screen = pygame.display.set_mode((WIDTH, HEIGHT))
    pygame.display.set_caption('Lab 4 - Composite Transformations')
    title = pygame.font.SysFont('consolas', 25, bold=True)
    font = pygame.font.SysFont('consolas', 18)

    shape = np.array([[-160,-80],[-40,-80],[-20,20],[-100,90],[-180,20]], float)
    composite = T(235,70) @ R(35) @ S(1.25,0.85)
    result = transform(shape, composite)

    running = True
    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
                running = False
        screen.fill(BG)
        pygame.draw.line(screen, (55,63,80), (0,HEIGHT//2+30), (WIDTH,HEIGHT//2+30), 1)
        pygame.draw.line(screen, (55,63,80), (WIDTH//2,100), (WIDTH//2,HEIGHT), 1)
        pygame.draw.polygon(screen, ORIGINAL, to_screen(shape), 3)
        pygame.draw.polygon(screen, RESULT, to_screen(result), 4)
        screen.blit(title.render('LAB 4 - COMPOSITE TRANSFORMATIONS USING MATRIX REPRESENTATION', True, TEXT), (25,25))
        screen.blit(font.render('M = T(235,70) x R(35 deg) x S(1.25,0.85)', True, (180,190,210)), (27,66))
        screen.blit(font.render('Original', True, ORIGINAL), (27,112))
        screen.blit(font.render('Composite result', True, RESULT), (145,112))
        pygame.display.flip()
    pygame.quit()


if __name__ == '__main__':
    main()
