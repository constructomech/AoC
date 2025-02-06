from collections import defaultdict
from itertools import combinations

def main():
    with open('input.txt', 'r') as f:
        lines = [line.strip() for line in f.readlines() if line.strip()]
    
    height = len(lines)
    if height == 0:
        print(0)
        return
    width = len(lines[0])
    
    antennas = []
    for y in range(height):
        line = lines[y]
        for x in range(len(line)):
            c = line[x]
            if c != '.':
                antennas.append((x, y, c))
    
    groups = defaultdict(list)
    for x, y, c in antennas:
        groups[c].append((x, y))
    
    antinodes = set()
    
    for freq in groups:
        same_freq = groups[freq]
        for a, b in combinations(same_freq, 2):
            ax, ay = a
            bx, by = b
            
            # Calculate antinode P: 2*B - A
            p_x = 2 * bx - ax
            p_y = 2 * by - ay
            if 0 <= p_x < width and 0 <= p_y < height:
                antinodes.add((p_x, p_y))
            
            # Calculate antinode Q: 2*A - B
            q_x = 2 * ax - bx
            q_y = 2 * ay - by
            if 0 <= q_x < width and 0 <= q_y < height:
                antinodes.add((q_x, q_y))
    
    print(len(antinodes))

if __name__ == "__main__":
    main()