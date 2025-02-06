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
            break
    if start_x is not None:
        break

# Directions: up, right, down, left
directions = [(0, -1), (1, 0), (0, 1), (-1, 0)]
width = len(grid[0])
height = len(grid)

count = 0

# Iterate over all possible candidate positions
for candidate_y in range(height):
    for candidate_x in range(width):
        # Skip starting position and existing obstructions
        if (candidate_x, candidate_y) == (start_x, start_y):
            continue
        if grid[candidate_y][candidate_x] == '#':
            continue
        
        # Simulate the guard's path with the new obstruction at (candidate_x, candidate_y)
        visited = set()
        current_x, current_y = start_x, start_y
        current_dir = direction
        loop_detected = False
        steps = 0
        max_steps = 4 * width * height
        
        while steps <= max_steps:
            state = (current_x, current_y, current_dir)
            if state in visited:
                loop_detected = True
                break
            visited.add(state)
            
            dx, dy = directions[current_dir]
            next_x = current_x + dx
            next_y = current_y + dy
            
            # Check if next step is out of bounds
            if not (0 <= next_x < width and 0 <= next_y < height):
                break
            
            # Check if next step is an obstruction (original or new)
            if grid[next_y][next_x] == '#' or (next_x == candidate_x and next_y == candidate_y):
                current_dir = (current_dir + 1) % 4
            else:
                current_x, current_y = next_x, next_y
            
            steps += 1
        
        if loop_detected:
            count += 1

print(count)