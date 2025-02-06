# Read input
with open('input.txt', 'r') as f:
    grid = [line.strip() for line in f]

dir_map = {'^': 0, '>': 1, 'v': 2, '<': 3}
start_x = start_y = direction = None

# Find the starting position and direction
for y in range(len(grid)):
    row = grid[y]
    for x in range(len(row)):
        c = row[x]
        if c in dir_map:
            start_x, start_y = x, y
            direction = dir_map[c]
            # Replace the symbol with '.' in the grid
            grid[y] = row[:x] + '.' + row[x+1:]
            break
    if start_x is not None:
        break

# Define movement directions (up, right, down, left)
directions = [(0, -1), (1, 0), (0, 1), (-1, 0)]
current_x, current_y = start_x, start_y
visited = set()
visited.add((current_x, current_y))

width = len(grid[0])
height = len(grid)

while True:
    dx, dy = directions[direction]
    next_x = current_x + dx
    next_y = current_y + dy

    # Check if the next cell is inside the grid and is an obstacle
    if 0 <= next_x < width and 0 <= next_y < height and grid[next_y][next_x] == '#':
        # Turn right
        direction = (direction + 1) % 4
    else:
        # Move to the next cell
        current_x, current_y = next_x, next_y
        # Check if the new position is outside the grid
        if not (0 <= current_x < width and 0 <= current_y < height):
            break
        visited.add((current_x, current_y))

print(len(visited))