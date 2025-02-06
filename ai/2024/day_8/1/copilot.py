import itertools

def read_map(filename):
    with open(filename, 'r') as f:
        grid = [list(line.rstrip('\n')) for line in f]
    return grid

def get_antennas(grid):
    antennas = {}
    for y, row in enumerate(grid):
        for x, char in enumerate(row):
            if char != '.':
                if char not in antennas:
                    antennas[char] = []
                antennas[char].append((x, y))
    return antennas

def compute_antinodes(antennas, grid_width, grid_height):
    antinodes = set()
    for freq, positions in antennas.items():
        # Generate all unordered pairs of antennas
        for (x1, y1), (x2, y2) in itertools.combinations(positions, 2):
            # Compute P1
            numerator_x = 2 * x1 + x2
            numerator_y = 2 * y1 + y2
            if numerator_x % 3 == 0 and numerator_y % 3 == 0:
                p1_x = numerator_x // 3
                p1_y = numerator_y // 3
                if 0 <= p1_x < grid_width and 0 <= p1_y < grid_height:
                    antinodes.add((p1_x, p1_y))
            # Compute P2
            numerator_x = x1 + 2 * x2
            numerator_y = y1 + 2 * y2
            if numerator_x % 3 == 0 and numerator_y % 3 == 0:
                p2_x = numerator_x // 3
                p2_y = numerator_y // 3
                if 0 <= p2_x < grid_width and 0 <= p2_y < grid_height:
                    antinodes.add((p2_x, p2_y))
            # Compute P3
            p3_x = 2 * x1 - x2
            p3_y = 2 * y1 - y2
            if 0 <= p3_x < grid_width and 0 <= p3_y < grid_height:
                antinodes.add((p3_x, p3_y))
            # Compute P4
            p4_x = -x1 + 2 * x2
            p4_y = -y1 + 2 * y2
            if 0 <= p4_x < grid_width and 0 <= p4_y < grid_height:
                antinodes.add((p4_x, p4_y))
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
