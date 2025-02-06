import math
from collections import defaultdict
import sys

def read_map(filename):
    with open(filename, 'r') as f:
        grid = [list(line.rstrip('\n')) for line in f]
    return grid

def get_antennas(grid):
    antennas = defaultdict(list)
    for y, row in enumerate(grid):
        for x, char in enumerate(row):
            if char != '.':
                antennas[char].append((x, y))
    return antennas

def normalize_line(A, B, C):
    gcd_val = math.gcd(math.gcd(abs(A), abs(B)), abs(C))
    A_n = A // gcd_val
    B_n = B // gcd_val
    C_n = C // gcd_val
    # Adjust signs for uniqueness
    if A_n < 0 or (A_n == 0 and B_n < 0):
        A_n *= -1
        B_n *= -1
        C_n *= -1
    return (A_n, B_n, C_n)

def compute_antinodes(antennas, grid_width, grid_height):
    antinodes = set()
    for freq, positions in antennas.items():
        if len(positions) < 2:
            continue  # No antinodes if only one antenna of this frequency
        lines = {}
        # Build lines between all pairs of antennas
        for i in range(len(positions)):
            x0, y0 = positions[i]
            for j in range(i + 1, len(positions)):
                x1, y1 = positions[j]
                dx = x1 - x0
                dy = y1 - y0
                if dx == 0 and dy == 0:
                    continue  # Same antenna
                # Line representation: A x + B y + C = 0
                A = dy
                B = -dx
                C = dx * y0 - dy * x0
                line_key = normalize_line(A, B, C)
                if line_key not in lines:
                    lines[line_key] = (dx, dy, x0, y0)
        # For each unique line, generate antinode positions
        for line_key, (dx, dy, x0, y0) in lines.items():
            g = math.gcd(dx, dy)
            dx_s = dx // g
            dy_s = dy // g
            # Compute t_min and t_max for x and y bounds
            t_values = []
            if dx_s != 0:
                t_min_x = (0 - x0) / dx_s
                t_max_x = (grid_width - 1 - x0) / dx_s
                t_values.append((math.ceil(min(t_min_x, t_max_x)), math.floor(max(t_min_x, t_max_x))))
            else:
                if 0 <= x0 < grid_width:
                    t_values.append((-sys.maxsize, sys.maxsize))
                else:
                    continue  # Line is outside grid
            if dy_s != 0:
                t_min_y = (0 - y0) / dy_s
                t_max_y = (grid_height - 1 - y0) / dy_s
                t_values.append((math.ceil(min(t_min_y, t_max_y)), math.floor(max(t_min_y, t_max_y))))
            else:
                if 0 <= y0 < grid_height:
                    t_values.append((-sys.maxsize, sys.maxsize))
                else:
                    continue  # Line is outside grid
            # Determine overall t_min and t_max
            t_min = max(interval[0] for interval in t_values)
            t_max = min(interval[1] for interval in t_values)
            # Generate positions along the line
            for t in range(t_min, t_max + 1):
                x = x0 + t * dx_s
                y = y0 + t * dy_s
                if 0 <= x < grid_width and 0 <= y < grid_height:
                    antinodes.add((x, y))
        # Add antenna positions themselves as antinodes
        for x, y in positions:
            antinodes.add((x, y))
    return antinodes

def main():
    grid = read_map('input.txt')
    grid_height = len(grid)
    grid_width = len(grid[0]) if grid else 0
    antennas = get_antennas(grid)
    antinodes = compute_antinodes(antennas, grid_width, grid_height)
    print(len(antinodes))

if __name__ == '__main__':
    main()
