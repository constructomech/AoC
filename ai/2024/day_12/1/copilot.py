# Read the garden plot map from input.txt
with open('input.txt', 'r') as f:
    grid = [list(line.strip()) for line in f if line.strip()]

rows = len(grid)
cols = len(grid[0]) if rows > 0 else 0

# Initialize a visited grid to keep track of processed plots
visited = [[False for _ in range(cols)] for _ in range(rows)]

# Directions for neighboring plots (up, down, left, right)
directions = [(-1, 0), (1, 0), (0, -1), (0, 1)]

total_price = 0  # Initialize total price

def dfs(i, j, plant_type):
    stack = [(i, j)]
    visited[i][j] = True
    area = 0
    perimeter = 0

    while stack:
        x, y = stack.pop()
        area += 1

        # Check all four sides of the current plot
        for dx, dy in directions:
            nx, ny = x + dx, y + dy

            # If the neighbor is out of bounds, it's an edge contributing to the perimeter
            if nx < 0 or nx >= rows or ny < 0 or ny >= cols:
                perimeter += 1
            elif grid[nx][ny] != plant_type:
                # If the neighbor is a different plant type, it contributes to the perimeter
                perimeter += 1
            else:
                # If the neighbor is the same plant type and not visited, add it to the stack
                if not visited[nx][ny]:
                    visited[nx][ny] = True
                    stack.append((nx, ny))

    return area, perimeter

# Iterate over every plot in the grid
for i in range(rows):
    for j in range(cols):
        if not visited[i][j]:
            plant_type = grid[i][j]
            area, perimeter = dfs(i, j, plant_type)
            price = area * perimeter
            total_price += price

# Output the total price
print(total_price)
