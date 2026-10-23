import pygame

WIDTH, HEIGHT = 900, 600
BG = (15, 18, 28)
GRID = (35, 42, 58)
LINE = (80, 220, 255)
POINT = (255, 210, 70)
TEXT = (235, 240, 250)


def dda(x1, y1, x2, y2):
    dx = x2 - x1
    dy = y2 - y1
    steps = int(max(abs(dx), abs(dy)))
    if steps == 0:
        return [(round(x1), round(y1))]
    x_inc = dx / steps
    y_inc = dy / steps
    x, y = x1, y1
    points = []
    for _ in range(steps + 1):
        points.append((round(x), round(y)))
        x += x_inc
        y += y_inc
    return points


def main():
    pygame.init()
    screen = pygame.display.set_mode((WIDTH, HEIGHT))
    pygame.display.set_caption('Lab 1 - DDA Line Drawing Algorithm')
    font = pygame.font.SysFont('consolas', 22)
    start, end = (120, 470), (780, 130)
    points = dda(*start, *end)
    running = True
    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
                running = False
        screen.fill(BG)
        for x in range(0, WIDTH, 50):
            pygame.draw.line(screen, GRID, (x, 0), (x, HEIGHT), 1)
        for y in range(0, HEIGHT, 50):
            pygame.draw.line(screen, GRID, (0, y), (WIDTH, y), 1)
        pygame.draw.line(screen, LINE, start, end, 2)
        for p in points[::8]:
            pygame.draw.circle(screen, POINT, p, 3)
        screen.blit(font.render('LAB 1 - DDA LINE DRAWING ALGORITHM', True, TEXT), (24, 20))
        screen.blit(font.render(f'Start: {start}   End: {end}   Generated pixels: {len(points)}', True, TEXT), (24, 52))
        pygame.display.flip()
    pygame.quit()


if __name__ == '__main__':
    main()
