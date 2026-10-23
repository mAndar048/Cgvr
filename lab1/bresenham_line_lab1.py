import pygame

WIDTH, HEIGHT = 900, 600
BG = (15, 18, 28)
GRID = (35, 42, 58)
LINE = (255, 100, 120)
POINT = (110, 255, 160)
TEXT = (235, 240, 250)


def bresenham(x1, y1, x2, y2):
    points = []
    dx, dy = abs(x2 - x1), abs(y2 - y1)
    sx = 1 if x1 < x2 else -1
    sy = 1 if y1 < y2 else -1
    err = dx - dy
    while True:
        points.append((x1, y1))
        if x1 == x2 and y1 == y2:
            break
        e2 = 2 * err
        if e2 > -dy:
            err -= dy
            x1 += sx
        if e2 < dx:
            err += dx
            y1 += sy
    return points


def main():
    pygame.init()
    screen = pygame.display.set_mode((WIDTH, HEIGHT))
    pygame.display.set_caption('Lab 1 - Bresenham Line Drawing Algorithm')
    font = pygame.font.SysFont('consolas', 22)
    start, end = (120, 140), (790, 470)
    points = bresenham(*start, *end)
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
        screen.blit(font.render('LAB 1 - BRESENHAM LINE DRAWING ALGORITHM', True, TEXT), (24, 20))
        screen.blit(font.render(f'Start: {start}   End: {end}   Generated pixels: {len(points)}', True, TEXT), (24, 52))
        pygame.display.flip()
    pygame.quit()


if __name__ == '__main__':
    main()
