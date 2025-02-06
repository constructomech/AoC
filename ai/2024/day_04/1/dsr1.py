# Read the input grid
with open('input.txt', 'r') as f:
    grid = [line.strip() for line in f]

rows = len(grid)
if rows == 0:
    print(0)
    exit()
cols = len(grid[0])

# Define all 8 possible directions
directions = [(-1, -1), (-1, 0), (-1, 1),
              (0, -1),          (0, 1),
              (1, -1),  (1, 0), (1, 1)]

count = 0

for i in range(rows):
    for j in range(cols):
        if grid[i][j] == 'X':
            for dx, dy in directions:
                valid = True
                # Check if the next three steps are within bounds
                for step in 1, 2, 3:
                    ni = i + dx * step
                    nj = j + dy * step
                    if ni < 0 or ni >= rows or nj < 0 or nj >= cols:
                        valid = False
                        break
                if valid:
                    # Check the characters at the positions
                    if (grid[i + dx][j + dy] == 'M' and
                        grid[i + 2*dx][j + 2*dy] == 'A' and
                        grid[i + 3*dx][j + 3*dy] == 'S'):
                        count += 1

print(count)