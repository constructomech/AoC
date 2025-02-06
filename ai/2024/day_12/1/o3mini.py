def read_input(filename):
    with open(filename) as f:
        return [list(line.rstrip("\n")) for line in f]

def compute_region(i, j, grid, visited):
    rows = len(grid)
    cols = len(grid[0])
    letter = grid[i][j]
    area = 0
    perimeter = 0
    stack = [(i, j)]
    while stack:
        r, c = stack.pop()
        if (r, c) in visited:
            continue
        visited.add((r, c))
        area += 1
        # For each of the four directions, check if the side is a boundary.
        for dr, dc in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
            nr, nc = r + dr, c + dc
            # if the neighbor is off the grid, add to perimeter
            if nr < 0 or nr >= rows or nc < 0 or nc >= cols:
                perimeter += 1
            # if the neighbor is within the grid but not the same letter, add to perimeter
            elif grid[nr][nc] != letter:
                perimeter += 1
            # if the neighbor is the same letter and not visited, add to the stack
            elif (nr, nc) not in visited:
                stack.append((nr, nc))
    return area, perimeter

def total_fence_price(grid):
    visited = set()
    total_price = 0
    rows = len(grid)
    cols = len(grid[0])
    for i in range(rows):
        for j in range(cols):
            if (i, j) not in visited:
                area, perimeter = compute_region(i, j, grid, visited)
                total_price += area * perimeter
    return total_price

if __name__ == '__main__':
    grid = read_input("input.txt")
    result = total_fence_price(grid)
    print("Total price of fencing:", result)
