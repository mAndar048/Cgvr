import pygame

WIDTH, HEIGHT = 800, 650
BG = (14, 17, 27)
PIXEL = (70, 220, 255)
CENTER = (255, 200, 70)
TEXT = (238, 242, 250)


def midpoint_circle(cx, cy, radius):
    x, y = 0, radius
    p = 1 - radius
    points = []
    while x <= y:
        points.extend([
            (cx+x, cy+y), (cx-x, cy+y), (cx+x, cy-y), (cx-x, cy-y),
            (cx+y, cy+x), (cx-y, cy+x), (cx+y, cy-x), (cx-y, cy-x)
        ])
        x += 1
        if p < 0:
            p += 2*x + 1
        else:
            y -= 1
            p += 2*(x-y) + 1
    return points


def main():
    pygame.init()
    screen = pygame.display.set_mode((WIDTH, HEIGHT))
    pygame.display.set_caption('Lab 2 - Midpoint Circle Drawing Algorithm')
    font = pygame.font.SysFont('consolas', 22)
    cx, cy, r = WIDTH//2, HEIGHT//2 + 25, 205
    points = midpoint_circle(cx, cy, r)
    running = True
    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
                running = False
        screen.fill(BG)
        for p in points:
            if 0 <= p[0] < WIDTH and 0 <= p[1] < HEIGHT:
                screen.set_at(p, PIXEL)
        pygame.draw.circle(screen, CENTER, (cx, cy), 5)
        screen.blit(font.render('LAB 2 - MIDPOINT CIRCLE DRAWING ALGORITHM', True, TEXT), (26, 22))
        screen.blit(font.render(f'Center = ({cx}, {cy})    Radius = {r}', True, TEXT), (26, 55))
        pygame.display.flip()
    pygame.quit()


if __name__ == '__main__':
    main()
