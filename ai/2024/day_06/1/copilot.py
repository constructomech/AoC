# Read the map from input.txt
with open('input.txt', 'r') as f:
    grid = [list(line.rstrip('\n')) for line in f]

height = len(grid)
width = len(grid[0]) if height > 0 else 0

# Define the directions: up, right, down, left
directions = [(-1, 0), (0, 1), (1, 0), (0, -1)]  # (dx, dy)
direction_symbols = {'^': 0, '>': 1, 'v': 2, '<': 3}

# Find the guard's starting position and facing direction
for x in range(height):
    for y in range(width):
        cell = grid[x][y]
        if cell in direction_symbols:
            start_x, start_y = x, y
            facing = direction_symbols[cell]
            grid[x][y] = '.'  # Replace the starting symbol with empty space
            break
    else:
        continue
    break

# Initialize visited positions
visited = set()
visited.add((start_x, start_y))

# Simulate the guard's movement
x, y = start_x, start_y

while True:
    dx, dy = directions[facing]
    nx, ny = x + dx, y + dy

    # Check if the position in front is within bounds and has an obstacle
    if 0 <= nx < height and 0 <= ny < width and grid[nx][ny] == '#':
        # Turn right 90 degrees
        facing = (facing + 1) % 4
    else:
        # Move forward
        x, y = nx, ny

        # Check if the new position is outside the map bounds
        if not (0 <= x < height and 0 <= y < width):
            break  # Guard has left the mapped area

        # Now that we've confirmed the new position is within bounds, add it to visited
        visited.add((x, y))

# Output the number of distinct positions visited
print(len(visited))
