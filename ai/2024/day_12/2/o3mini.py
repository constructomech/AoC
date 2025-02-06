def read_input(filename):
    """Read the puzzle input into a 2D grid (list of lists of single‐character strings)."""
    with open(filename) as f:
        return [list(line.rstrip("\n")) for line in f]

def flood_fill(i, j, grid, visited):
    """
    Perform DFS starting at cell (i,j) to collect all cells in the region.
    Returns a set of (row, col) coordinates for the region.
    """
    n = len(grid)
    m = len(grid[0])
    letter = grid[i][j]
    region = set()
    stack = [(i, j)]
    while stack:
        r, c = stack.pop()
        if (r, c) in visited:
            continue
        visited.add((r, c))
        region.add((r, c))
        for dr, dc in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
            nr, nc = r + dr, c + dc
            if 0 <= nr < n and 0 <= nc < m:
                if grid[nr][nc] == letter and (nr, nc) not in visited:
                    stack.append((nr, nc))
    return region

def merge_intervals(intervals):
    """
    Given a list of intervals (each a tuple (start, end)),
    merge contiguous intervals and return the number of merged intervals.
    Two intervals [a, b] and [b, c] merge into [a, c].
    """
    if not intervals:
        return 0
    # sort by the starting coordinate
    intervals.sort(key=lambda x: x[0])
    merged = []
    current_start, current_end = intervals[0]
    for start, end in intervals[1:]:
        if start == current_end:  # contiguous
            current_end = end
        else:
            merged.append((current_start, current_end))
            current_start, current_end = start, end
    merged.append((current_start, current_end))
    return len(merged)

def compute_fence_sides(region, grid):
    """
    Given a set of cells (tuples (r, c)) in a region and the grid,
    compute the total number of distinct straight fence sides along the region's boundary.
    
    Each fence side is a maximal contiguous segment along a grid line.
    We consider horizontal fence segments (which lie along grid lines with constant row)
    and vertical fence segments (constant column) separately.
    """
    n = len(grid)
    m = len(grid[0])
    # We’ll record horizontal segments as intervals along a given grid row.
    # For a horizontal segment, we record the grid line row and the [col, col+1] interval.
    horizontal_segments = {}  # key: row (grid line), value: list of intervals (start_col, end_col)
    # Similarly, vertical segments are along a constant column (grid line).
    vertical_segments = {}    # key: column (grid line), value: list of intervals (start_row, end_row)
    
    # For each cell in the region, examine its four edges.
    for r, c in region:
        # Top edge: lies along grid line at row r, from col c to c+1.
        # Include it if we're at the top of the grid OR the cell above is not in the region.
        if r == 0 or ((r - 1, c) not in region):
            horizontal_segments.setdefault(r, []).append((c, c+1))
        # Bottom edge: along grid line at row r+1.
        if r == n - 1 or ((r + 1, c) not in region):
            horizontal_segments.setdefault(r+1, []).append((c, c+1))
        # Left edge: vertical edge along grid line at column c, from row r to r+1.
        if c == 0 or ((r, c - 1) not in region):
            vertical_segments.setdefault(c, []).append((r, r+1))
        # Right edge: vertical edge along grid line at column c+1.
        if c == m - 1 or ((r, c + 1) not in region):
            vertical_segments.setdefault(c+1, []).append((r, r+1))
    
    # Now merge contiguous segments along each grid line.
    sides = 0
    for row, intervals in horizontal_segments.items():
        sides += merge_intervals(intervals)
    for col, intervals in vertical_segments.items():
        sides += merge_intervals(intervals)
    
    return sides

def total_bulk_price(grid):
    """
    For each region in the grid, compute the price as:
         price = (area of region) * (number of fence sides)
    Then return the total price over all regions.
    """
    visited = set()
    total_price = 0
    n = len(grid)
    m = len(grid[0])
    for i in range(n):
        for j in range(m):
            if (i, j) not in visited:
                # Find the entire region using flood fill.
                region = flood_fill(i, j, grid, visited)
                area = len(region)
                sides = compute_fence_sides(region, grid)
                region_price = area * sides
                total_price += region_price
    return total_price

if __name__ == '__main__':
    grid = read_input("input.txt")
    result = total_bulk_price(grid)
    print("Total price of fencing (bulk discount):", result)
