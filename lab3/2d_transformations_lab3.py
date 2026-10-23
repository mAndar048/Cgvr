import math
import pygame
import numpy as np

WIDTH, HEIGHT = 1100, 720
BG = (14, 17, 27)
TEXT = (238, 242, 250)


def translation(tx, ty):
    return np.array([[1,0,tx],[0,1,ty],[0,0,1]], float)


def rotation(deg):
    a = math.radians(deg)
    c, s = math.cos(a), math.sin(a)
    return np.array([[c,-s,0],[s,c,0],[0,0,1]], float)


def scaling(sx, sy):
    return np.array([[sx,0,0],[0,sy,0],[0,0,1]], float)


def reflection_x():
    return np.array([[1,0,0],[0,-1,0],[0,0,1]], float)


def shearing(shx, shy):
    return np.array([[1,shx,0],[shy,1,0],[0,0,1]], float)


def apply(points, matrix):
    hom = np.column_stack((points, np.ones(len(points))))
    return (matrix @ hom.T).T[:, :2]


def to_screen(points, origin):
    ox, oy = origin
    return [(int(ox+x), int(oy-y)) for x, y in points]


def draw_panel(screen, font, title, pts, color, rect):
    pygame.draw.rect(screen, (30,36,52), rect, border_radius=12)
    pygame.draw.rect(screen, (65,75,98), rect, 2, border_radius=12)
    cx, cy = rect.center
    pygame.draw.line(screen, (70,76,92), (rect.left+15, cy), (rect.right-15, cy), 1)
    pygame.draw.line(screen, (70,76,92), (cx, rect.top+45), (cx, rect.bottom-15), 1)
    pygame.draw.polygon(screen, color, to_screen(pts, (cx, cy)), 3)
    screen.blit(font.render(title, True, color), (rect.left+14, rect.top+12))


def main():
    pygame.init()
    screen = pygame.display.set_mode((WIDTH, HEIGHT))
    pygame.display.set_caption('Lab 3 - 2D Transformations using Homogeneous Coordinates')
    title_font = pygame.font.SysFont('consolas', 24, bold=True)
    font = pygame.font.SysFont('consolas', 18)

    shape = np.array([[-55,-40],[65,-40],[35,55],[-35,75]], float)
    variants = [
        ('Original', shape, (240,240,240)),
        ('Translation', apply(shape, translation(50,35)), (70,220,255)),
        ('Rotation 40 deg', apply(shape, rotation(40)), (255,110,130)),
        ('Scaling 1.35 x 0.75', apply(shape, scaling(1.35,0.75)), (110,255,160)),
        ('Reflection about X', apply(shape, reflection_x()), (255,210,70)),
        ('Shearing', apply(shape, shearing(0.7,0.0)), (190,120,255)),
    ]

    rects = [pygame.Rect(25+c*355, 105+r*290, 330, 260) for r in range(2) for c in range(3)]
    running = True
    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
                running = False
        screen.fill(BG)
        screen.blit(title_font.render('LAB 3 - 2D TRANSFORMATIONS USING HOMOGENEOUS COORDINATES', True, TEXT), (28,30))
        screen.blit(font.render('Translation | Rotation | Scaling | Reflection | Shearing', True, (180,190,210)), (30,68))
        for rect, item in zip(rects, variants):
            draw_panel(screen, font, *item, rect)
        pygame.display.flip()
    pygame.quit()


if __name__ == '__main__':
    main()
