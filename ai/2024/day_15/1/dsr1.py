# Read input
lines = [line.rstrip('\n') for line in open('input.txt')]

# Split into warehouse and moves
warehouse = []
moves = []
found = False

for i, line in enumerate(lines):
    if any(c in '<>^v' for c in line):
        warehouse = lines[:i]
        moves = lines[i:]
        found = True
        break

if not found:
    warehouse = lines

# Join moves into a single string, stripping any newlines
moves_str = ''.join(moves).strip()

# Parse the warehouse grid and find the robot's initial position
grid = []
robot_x, robot_y = -1, -1

for y, line in enumerate(warehouse):
    if line.strip() == '':
        continue
    grid.append(list(line))
    for x, c in enumerate(grid[y]):
        if c == '@':
            robot_x, robot_y = x, y

# Directions mapping
directions = {
    '^': (0, -1),
    'v': (0, 1),
    '<': (-1, 0),
    '>': (1, 0)
}

# Function to check if coordinates are within bounds
def is_valid(x, y):
    return 0 <= y < len(grid) and 0 <= x < len(grid[0])

# Process each move
for move in moves_str:
    if move not in directions:
        continue  # skip invalid characters
    dx, dy = directions[move]
    target_x = robot_x + dx
    target_y = robot_y + dy
    
    # Check if target is out of bounds or a wall
    if not is_valid(target_x, target_y) or grid[target_y][target_x] == '#':
        continue
    
    cell = grid[target_y][target_x]
    
    if cell == '.':
        # Move robot to empty cell
        grid[robot_y][robot_x] = '.'
        grid[target_y][target_x] = '@'
        robot_x, robot_y = target_x, target_y
    elif cell == 'O':
        # Check consecutive boxes
        k = 0
        check_x, check_y = target_x, target_y
        while is_valid(check_x, check_y) and grid[check_y][check_x] == 'O':
            k += 1
            check_x += dx
            check_y += dy
        
        # Determine next cell after boxes
        next_cell_x = target_x + k * dx
        next_cell_y = target_y + k * dy
        
        # Check if next cell is valid and can be moved into
        if not is_valid(next_cell_x, next_cell_y) or grid[next_cell_y][next_cell_x] in ('#', 'O'):
            continue  # can't push boxes
        
        # Push boxes
        for i in reversed(range(k)):
            current_x = target_x + i * dx
            current_y = target_y + i * dy
            next_x = current_x + dx
            next_y = current_y + dy
            grid[next_y][next_x] = 'O'
            grid[current_y][current_x] = '.'
        
        # Move robot into target cell
        grid[robot_y][robot_x] = '.'
        grid[target_y][target_x] = '@'
        robot_x, robot_y = target_x, target_y

# Calculate sum of GPS coordinates
sum_gps = 0
for y in range(len(grid)):
    for x in range(len(grid[y])):
        if grid[y][x] == 'O':
            sum_gps += 100 * y + x

print(sum_gps)