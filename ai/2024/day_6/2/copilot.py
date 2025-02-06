# guard_loop_deterministic.py

# Read the input map
with open('input.txt', 'r') as f:
    grid_content = [line.rstrip('\n') for line in f]

height = len(grid_content)
width = len(grid_content[0]) if height > 0 else 0

# Original grid, to reset for each simulation
original_grid = [list(line) for line in grid_content]

# Directions: up, right, down, left
directions = [(-1, 0), (0, 1), (1, 0), (0, -1)]
direction_symbols = {'^': 0, '>': 1, 'v': 2, '<': 3}

# Find the guard's starting position and facing direction
start_x = start_y = start_facing = None
for x in range(height):
    for y in range(width):
        cell = original_grid[x][y]
        if cell in direction_symbols:
            start_x, start_y = x, y
            start_facing = direction_symbols[cell]
            original_grid[x][y] = '.'  # Replace the starting symbol with empty space
            break
    if start_x is not None:
        break

if start_x is None:
    print("Guard starting position not found")
    exit(1)

# Identify possible obstruction positions (excluding starting position)
possible_positions = [
    (x, y) for x in range(height) for y in range(width)
    if original_grid[x][y] == '.' and not (x == start_x and y == start_y)
]

# Counter for obstruction positions that cause the guard to get stuck
loop_positions_count = 0

# Simulate for each possible obstruction
for obs_x, obs_y in possible_positions:
    # Reset the grid for each simulation
    grid = [row.copy() for row in original_grid]
    # Place the obstruction
    grid[obs_x][obs_y] = '#'

    # Initialize guard's state
    x, y = start_x, start_y
    facing = start_facing
    visited_states = set()

    loop_detected = False

    while True:
        state = (x, y, facing)
        if state in visited_states:
            # Guard is stuck in a loop
            loop_detected = True
            break
        visited_states.add(state)

        dx, dy = directions[facing]
        nx, ny = x + dx, y + dy

        # Check if the new position is within bounds
        if 0 <= nx < height and 0 <= ny < width:
            if grid[nx][ny] == '#':
                # Obstacle ahead; turn right
                facing = (facing + 1) % 4
            else:
                # Move forward
                x, y = nx, ny
        else:
            # Guard has left the mapped area
            break

    if loop_detected:
        loop_positions_count += 1

# Output the result
print(loop_positions_count)
