from collections import deque

with open('input.txt') as f:
    grid = [line.strip() for line in f]

rows = len(grid)
cols = len(grid[0]) if rows else 0

visited = [[False for _ in range(cols)] for _ in range(rows)]
regions = []

for i in range(rows):
    for j in range(cols):
        if not visited[i][j]:
            q = deque()
            q.append((i, j))
            visited[i][j] = True
            current_char = grid[i][j]
            current_region = set()
            current_region.add((i, j))
            while q:
                x, y = q.popleft()
                for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
                    nx = x + dx
                    ny = y + dy
                    if 0 <= nx < rows and 0 <= ny < cols:
                        if not visited[nx][ny] and grid[nx][ny] == current_char:
                            visited[nx][ny] = True
                            q.append((nx, ny))
                            current_region.add((nx, ny))
            regions.append(current_region)

total = 0

for region in regions:
    area = len(region)
    perimeter = 0
    for (i, j) in region:
        for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
            ni = i + dx
            nj = j + dy
            if ni < 0 or ni >= rows or nj < 0 or nj >= cols:
                perimeter += 1
            else:
                if (ni, nj) not in region:
                    perimeter += 1
    total += area * perimeter

print(total)