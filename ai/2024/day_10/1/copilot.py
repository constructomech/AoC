# Read the topographic map from 'input.txt'
with open('input.txt', 'r') as file:
    lines = [line.strip() for line in file if line.strip()]

# Build the grid
grid = [list(map(int, list(line))) for line in lines]
rows = len(grid)
cols = len(grid[0])

# Directions for movement: up, down, left, right
directions = [(-1, 0), (1, 0), (0, -1), (0, 1)]

# Find all trailheads (positions with height 0)
trailheads = []
for x in range(rows):
    for y in range(cols):
        if grid[x][y] == 0:
            trailheads.append((x, y))

total_score = 0

for trailhead in trailheads:
    from collections import deque

    # BFS initialization
    queue = deque()
    queue.append((trailhead[0], trailhead[1], 0))  # (x, y, current_height)
    visited = set()
    visited.add((trailhead[0], trailhead[1]))
    reached_nines = set()

    while queue:
        x, y, current_height = queue.popleft()

        # If we have reached height 9, record the position
        if grid[x][y] == 9:
            reached_nines.add((x, y))
            continue  # No need to explore further from here

        # Explore neighbors
        for dx, dy in directions:
            nx, ny = x + dx, y + dy

            # Check boundaries
            if 0 <= nx < rows and 0 <= ny < cols:
                # Next height should be exactly current height + 1
                if grid[nx][ny] == grid[x][y] + 1:
                    if (nx, ny) not in visited:
                        visited.add((nx, ny))
                        queue.append((nx, ny, grid[nx][ny]))

    # The score for this trailhead is the number of unique height-9 positions reached
    score = len(reached_nines)
    total_score += score

# Output the total score
print(total_score)
