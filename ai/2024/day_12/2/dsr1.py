from collections import deque, defaultdict

with open('input.txt') as f:
    grid = [line.strip() for line in f]

rows = len(grid)
cols = len(grid[0]) if rows else 0

visited = [[False for _ in range(cols)] for _ in range(rows)]
regions = []

# Identify all regions using BFS
for i in range(rows):
    for j in range(cols):
        if not visited[i][j]:
            q = deque()
            q.append((i, j))
            visited[i][j] = True
            current_char = grid[i][j]
            current_region = set()
            current_region.add((i, j))
            while q:
                x, y = q.popleft()
                for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
                    nx, ny = x + dx, y + dy
                    if 0 <= nx < rows and 0 <= ny < cols:
                        if not visited[nx][ny] and grid[nx][ny] == current_char:
                            visited[nx][ny] = True
                            q.append((nx, ny))
                            current_region.add((nx, ny))
            regions.append(current_region)

total = 0

# Process each region to calculate the number of sides and total price
for region in regions:
    h_edges = defaultdict(list)  # Key: y, horizontal edges are stored as (x_start, x_end)
    v_edges = defaultdict(list)  # Key: x, vertical edges are stored as (y_start, y_end)
    area = len(region)
    region_set = region

    # Collect perimeter edges
    for (i, j) in region_set:
        # Check top edge (i-1, j)
        if i == 0 or (i-1, j) not in region_set:
            h_edges[i].append((j, j + 1))
        # Check bottom edge (i+1, j)
        if i == rows - 1 or (i+1, j) not in region_set:
            h_edges[i + 1].append((j, j + 1))
        # Check left edge (i, j-1)
        if j == 0 or (i, j-1) not in region_set:
            v_edges[j].append((i, i + 1))
        # Check right edge (i, j+1)
        if j == cols - 1 or (i, j+1) not in region_set:
            v_edges[j + 1].append((i, i + 1))

    # Function to merge contiguous segments
    def merge_segments(segments):
        if not segments:
            return []
        segments.sort()
        merged = [segments[0]]
        for s in segments[1:]:
            last = merged[-1]
            if s[0] <= last[1]:
                merged[-1] = (last[0], max(last[1], s[1]))
            else:
                merged.append(s)
        return merged

    sides = 0

    # Merge horizontal edges
    for y in h_edges:
        merged = merge_segments(h_edges[y])
        sides += len(merged)

    # Merge vertical edges
    for x in v_edges:
        merged = merge_segments(v_edges[x])
        sides += len(merged)

    total += area * sides

print(total)