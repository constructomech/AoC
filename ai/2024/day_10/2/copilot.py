# Read the topographic map from 'input.txt'
with open('input.txt', 'r') as file:
    raw_lines = [line.rstrip('\n') for line in file if line.strip()]

# Build the grid with proper handling of impassable tiles ('.')
grid = []
for line in raw_lines:
    row = []
    for char in line:
        if char == '.':
            row.append(None)  # None represents an impassable tile
        else:
            row.append(int(char))
    grid.append(row)

rows = len(grid)
cols = len(grid[0])

# Find all trailheads (positions with height 0)
trailheads = []
for x in range(rows):
    for y in range(cols):
        if grid[x][y] == 0:
            trailheads.append((x, y))

# Directions for movement: up, down, left, right
directions = [(-1, 0), (1, 0), (0, -1), (0, 1)]

total_rating = 0

for trailhead in trailheads:
    # Initialize counts grid for this trailhead
    counts = [[0 for _ in range(cols)] for _ in range(rows)]
    x0, y0 = trailhead
    counts[x0][y0] = 1  # Start with one path at the trailhead

    # Process heights from 0 to 8
    for height in range(0, 9):
        # Positions at the current height
        positions = []
        for x in range(rows):
            for y in range(cols):
                if grid[x][y] == height and counts[x][y] > 0:
                    positions.append((x, y))

        # For each position, propagate paths to adjacent positions of height +1
        for x, y in positions:
            for dx, dy in directions:
                nx, ny = x + dx, y + dy
                # Check boundaries and impassable tiles
                if 0 <= nx < rows and 0 <= ny < cols:
                    if grid[nx][ny] is not None:
                        if grid[nx][ny] == height + 1:
                            counts[nx][ny] += counts[x][y]

    # Calculate the rating for this trailhead
    rating = 0
    for x in range(rows):
        for y in range(cols):
            if grid[x][y] == 9 and counts[x][y] > 0:
                rating += counts[x][y]

    total_rating += rating

# Output the total rating
print(total_rating)


