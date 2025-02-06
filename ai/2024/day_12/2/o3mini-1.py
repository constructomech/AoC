def read_input(filename):
    """Read the puzzle input into a 2D grid (list of lists of single-character strings)."""
    with open(filename) as f:
        return [list(line.rstrip("\n")) for line in f]

def flood_fill(i, j, grid, visited):
    """
    Do a DFS from (i,j) collecting all cells (r,c) that are in the same region
    (neighbors up/down/left/right with the same letter).
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
        for dr, dc in [(-1,0),(1,0),(0,-1),(0,1)]:
            nr, nc = r+dr, c+dc
            if 0 <= nr < n and 0 <= nc < m:
                if grid[nr][nc] == letter and (nr, nc) not in visited:
                    stack.append((nr, nc))
    return region

def merge_intervals(intervals):
    """
    Given a list of intervals (each a tuple (start, end)), merge contiguous ones.
    Two intervals [a,b] and [b,c] merge to [a,c]. Return the number of merged intervals.
    """
    if not intervals:
        return 0
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
    Compute the number of distinct straight fence sides for the given region.
    Now, we keep track of the "side" (which tells us whether the fence is coming from
    the top or bottom of a cell for horizontal edges, or left/right for vertical edges).
    
    Each fence side is a maximal contiguous segment along a grid line that comes from the same side.
    """
    n = len(grid)
    m = len(grid[0])
    # Use dictionaries keyed by (line, side) rather than just the line.
    horizontal_segments = {}  # key: (row, "top") or (row, "bottom"), value: list of (start, end)
    vertical_segments = {}    # key: (col, "left") or (col, "right"), value: list of (start, end)
    
    for r, c in region:
        # Check the top edge of cell (r,c):
        # It lies along grid line at row r from c to c+1.
        # Include it if we are at the top of the grid OR the cell above is not in the region.
        if r == 0 or ((r-1, c) not in region):
            key = (r, "top")
            horizontal_segments.setdefault(key, []).append((c, c+1))
        # Bottom edge:
        if r == n-1 or ((r+1, c) not in region):
            key = (r+1, "bottom")
            horizontal_segments.setdefault(key, []).append((c, c+1))
        # Left edge:
        if c == 0 or ((r, c-1) not in region):
            key = (c, "left")
            vertical_segments.setdefault(key, []).append((r, r+1))
        # Right edge:
        if c == m-1 or ((r, c+1) not in region):
            key = (c+1, "right")
            vertical_segments.setdefault(key, []).append((r, r+1))
    
    # Now merge contiguous segments for each (grid_line, side) group.
    sides = 0
    for key, intervals in horizontal_segments.items():
        sides += merge_intervals(intervals)
    for key, intervals in vertical_segments.items():
        sides += merge_intervals(intervals)
    return sides

def total_bulk_price(grid):
    """
    For each region, compute its price as:
         price = (area of region) * (number of fence sides)
    and return the sum over all regions.
    """
    visited = set()
    total_price = 0
    n = len(grid)
    m = len(grid[0])
    for i in range(n):
        for j in range(m):
            if (i,j) not in visited:
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
