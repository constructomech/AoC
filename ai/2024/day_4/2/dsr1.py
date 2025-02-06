# Read the input grid
with open('input.txt', 'r') as f:
    grid = [line.strip() for line in f]

rows = len(grid)
if rows == 0:
    print(0)
    exit()
cols = len(grid[0])

count = 0

for i in range(rows):
    for j in range(cols):
        if grid[i][j] == 'A':
            # Check if the 'A' can be the center of a 3x3 grid
            if i < 1 or i >= rows - 1 or j < 1 or j >= cols - 1:
                continue
            # Check first diagonal (top-left to bottom-right)
            top_left = grid[i-1][j-1]
            bottom_right = grid[i+1][j+1]
            diag1_valid = (top_left == 'M' and bottom_right == 'S') or (top_left == 'S' and bottom_right == 'M')
            
            # Check second diagonal (top-right to bottom-left)
            top_right = grid[i-1][j+1]
            bottom_left = grid[i+1][j-1]
            diag2_valid = (top_right == 'M' and bottom_left == 'S') or (top_right == 'S' and bottom_left == 'M')
            
            if diag1_valid and diag2_valid:
                count += 1

print(count)