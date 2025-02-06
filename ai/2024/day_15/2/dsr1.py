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

# Scale the warehouse according to the problem's rules
scaled_warehouse = []
for line in warehouse:
    scaled_line = []
    for c in line:
        if c == '#':
            scaled_line.extend(['#', '#'])
        elif c == 'O':
            scaled_line.extend(['[', ']'])
        elif c == '.':
            scaled_line.extend(['.', '.'])
        elif c == '@':
            scaled_line.extend(['@', '.'])
        else:
            scaled_line.extend([c, c])
    scaled_warehouse.append(scaled_line)

# Find the robot's initial position in the scaled warehouse
robot_x, robot_y = -1, -1
for y in range(len(scaled_warehouse)):
    row = scaled_warehouse[y]
    for x in range(len(row)):
        if row[x] == '@':
            robot_x, robot_y = x, y
            break
    if robot_x != -1:
        break

# Directions mapping
directions = {
    '^': (0, -1),
    'v': (0, 1),
    '<': (-1, 0),
    '>': (1, 0)
}

def is_valid(x, y):
    return 0 <= y < len(scaled_warehouse) and 0 <= x < len(scaled_warehouse[y])

# Process each move
for move in moves_str:
    if move not in directions:
        continue  # skip invalid characters
    dx, dy = directions[move]
    tx = robot_x + dx
    ty = robot_y + dy
    
    # Check if target is out of bounds
    if not is_valid(tx, ty):
        continue
    
    cell = scaled_warehouse[ty][tx]
    
    if cell == '#':
        continue  # wall, can't move
    
    if cell == '.':
        # Move robot to empty cell
        scaled_warehouse[robot_y][robot_x] = '.'
        scaled_warehouse[ty][tx] = '@'
        robot_x, robot_y = tx, ty
    elif cell in ('[', ']'):
        # Determine the start of the box
        if cell == ']':
            start_x = tx - 1
            start_y = ty
        else:
            start_x = tx
            start_y = ty
        
        # Check if start_x and start_x+1 form a valid box
        if not is_valid(start_x, start_y) or not is_valid(start_x + 1, start_y):
            continue
        if scaled_warehouse[start_y][start_x] != '[' or scaled_warehouse[start_y][start_x + 1] != ']':
            continue
        
        # Determine the number of consecutive boxes in the direction of movement
        k = 1
        current_x, current_y = start_x, start_y
        while True:
            # Move in the direction of the move to check for next box
            if dx != 0:
                next_x = current_x + dx * 2
                next_y = current_y
            else:
                next_x = current_x
                next_y = current_y + dy * 2
            
            if not is_valid(next_x, next_y) or not is_valid(next_x + 1, next_y):
                break
            if scaled_warehouse[next_y][next_x] == '[' and scaled_warehouse[next_y][next_x + 1] == ']':
                k += 1
                current_x, current_y = next_x, next_y
            else:
                break
        
        # Check if all new positions are valid and empty
        can_move = True
        for i in range(k):
            if dx != 0:
                new_x = start_x + dx * (i + 1)
                new_y = start_y
            else:
                new_x = start_x
                new_y = start_y + dy * (i + 1)
            
            if not is_valid(new_x, new_y) or not is_valid(new_x + 1, new_y):
                can_move = False
                break
            if scaled_warehouse[new_y][new_x] != '.' or scaled_warehouse[new_y][new_x + 1] != '.':
                can_move = False
                break
        
        if not can_move:
            continue
        
        # Move all boxes in reverse order to avoid overwriting
        for i in reversed(range(k)):
            if dx != 0:
                old_x = start_x + dx * 2 * i
                old_y = start_y
                new_x = old_x + dx
                new_y = old_y
            else:
                old_x = start_x
                old_y = start_y + dy * 2 * i
                new_x = old_x
                new_y = old_y + dy
            
            # Move the box
            scaled_warehouse[new_y][new_x] = '['
            scaled_warehouse[new_y][new_x + 1] = ']'
            scaled_warehouse[old_y][old_x] = '.'
            scaled_warehouse[old_y][old_x + 1] = '.'
        
        # Move the robot into the target cell
        scaled_warehouse[robot_y][robot_x] = '.'
        scaled_warehouse[ty][tx] = '@'
        robot_x, robot_y = tx, ty

# Calculate the sum of GPS coordinates for all boxes
sum_gps = 0
for y in range(len(scaled_warehouse)):
    row = scaled_warehouse[y]
    x = 0
    while x < len(row):
        if row[x] == '[' and x + 1 < len(row) and row[x + 1] == ']':
            sum_gps += 100 * y + x
            x += 2  # Skip the next cell as it's part of the same box
        else:
            x += 1

print(sum_gps)