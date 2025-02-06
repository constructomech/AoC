from collections import deque

# Constants for movement directions
UP, RIGHT, DOWN, LEFT = 0, 1, 2, 3
directions = [(-1, 0), (0, 1), (1, 0), (0, -1)]
direction_names = ['UP', 'RIGHT', 'DOWN', 'LEFT']

def read_grid(filename):
    with open(filename, 'r') as f:
        grid = [list(line.strip()) for line in f if line.strip()]
    return grid

def get_neighbors(x, y, rows, cols):
    for dx, dy in directions:
        nx, ny = x + dx, y + dy
        yield nx, ny, dx, dy

def bfs(grid, visited, start_x, start_y, plant_type):
    rows, cols = len(grid), len(grid[0])
    queue = deque()
    queue.append((start_x, start_y))
    visited[start_x][start_y] = True
    area = 1
    boundary_edges = set()

    while queue:
        x, y = queue.popleft()

        for idx, (dx, dy) in enumerate(directions):
            nx, ny = x + dx, y + dy

            # Check if neighbor is within bounds
            if 0 <= nx < rows and 0 <= ny < cols:
                if grid[nx][ny] == plant_type:
                    if not visited[nx][ny]:
                        visited[nx][ny] = True
                        area += 1
                        queue.append((nx, ny))
                else:
                    # Boundary edge between (x, y) and (nx, ny)
                    boundary_edges.add((x, y, idx))
            else:
                # Edge is at the border of the grid
                boundary_edges.add((x, y, idx))

    sides = trace_boundary(boundary_edges)
    return area, sides

def trace_boundary(boundary_edges):
    # Build a map from each boundary cell to its edges
    edge_map = {}
    for x, y, dir_idx in boundary_edges:
        if (x, y) not in edge_map:
            edge_map[(x, y)] = []
        edge_map[(x, y)].append(dir_idx)

    # Find the starting point (boundary cell with the smallest coordinates)
    start_point = min(edge_map.keys())
    x, y = start_point

    # Initialize tracing variables
    current_cell = (x, y)
    direction_sequence = []
    visited_edges = set()

    # Start tracing from the edge with the lowest direction index
    initial_dir = min(edge_map[current_cell])
    current_dir = initial_dir
    direction_sequence.append(current_dir)
    visited_edges.add((current_cell, current_dir))

    while True:
        x, y = current_cell
        next_dir = (current_dir + 1) % 4  # Turn right
        found_next = False

        for i in range(4):
            dir_idx = (current_dir + i) % 4
            dx, dy = directions[dir_idx]
            nx, ny = x + dx, y + dy
            next_cell = (nx, ny)
            edge = (current_cell, dir_idx)

            if edge in visited_edges:
                continue

            if (x, y, dir_idx) in boundary_edges:
                # Edge exists; move to the next cell
                visited_edges.add(edge)
                current_dir = dir_idx
                direction_sequence.append(current_dir)
                current_cell = next_cell
                found_next = True
                break

        if not found_next or current_cell == start_point and current_dir == initial_dir:
            break

    # Simplify direction sequence by combining consecutive same directions
    simplified_directions = [direction_sequence[0]]
    for dir in direction_sequence[1:]:
        if dir != simplified_directions[-1]:
            simplified_directions.append(dir)

    number_of_sides = len(simplified_directions)
    return number_of_sides

def calculate_total_price(grid):
    rows, cols = len(grid), len(grid[0])
    visited = [[False]*cols for _ in range(rows)]
    total_price = 0

    for i in range(rows):
        for j in range(cols):
            if not visited[i][j]:
                plant_type = grid[i][j]
                area, sides = bfs(grid, visited, i, j, plant_type)
                price = area * sides
                total_price += price

    return total_price

if __name__ == "__main__":
    grid = read_grid('input.txt')
    total_price = calculate_total_price(grid)
    print(total_price)
