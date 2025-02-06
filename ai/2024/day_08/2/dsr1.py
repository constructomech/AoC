import math
from collections import defaultdict
from itertools import combinations

def compute_gcd(a, b, c):
    a_abs = abs(a)
    b_abs = abs(b)
    c_abs = abs(c)
    gcd_ab = math.gcd(a_abs, b_abs)
    gcd_abc = math.gcd(gcd_ab, c_abs)
    return gcd_abc

def normalize_line(A, B, C):
    g = compute_gcd(A, B, C)
    if g == 0:
        g = 1  # This case shouldn't occur as points are distinct
    A_norm = A // g
    B_norm = B // g
    C_norm = C // g

    # Determine the sign of the first non-zero coefficient
    first_non_zero = None
    for co in [A_norm, B_norm, C_norm]:
        if co != 0:
            first_non_zero = co
            break
    if first_non_zero is None:
        return (0, 0, 0)
    if first_non_zero < 0:
        A_norm *= -1
        B_norm *= -1
        C_norm *= -1
    return (A_norm, B_norm, C_norm)

def main():
    with open('input.txt', 'r') as f:
        lines = [line.strip() for line in f if line.strip()]
    
    if not lines:
        print(0)
        return
    
    height = len(lines)
    width = len(lines[0])
    
    antennas = []
    for y in range(height):
        line = lines[y]
        for x in range(width):
            c = line[x]
            if c != '.':
                antennas.append((x, y, c))
    
    groups = defaultdict(list)
    for x, y, c in antennas:
        groups[c].append((x, y))
    
    antinodes = set()
    
    for freq, positions in groups.items():
        if len(positions) < 2:
            continue  # Need at least two antennas to form a line
        
        unique_lines = set()
        
        for a, b in combinations(positions, 2):
            x1, y1 = a
            x2, y2 = b
            
            A = y2 - y1
            B = x1 - x2
            C = x2 * y1 - x1 * y2
            
            normalized = normalize_line(A, B, C)
            unique_lines.add(normalized)
        
        # Check each grid cell against all unique lines
        for line in unique_lines:
            A_line, B_line, C_line = line
            for x in range(width):
                for y in range(height):
                    if A_line * x + B_line * y + C_line == 0:
                        antinodes.add((x, y))
    
    print(len(antinodes))

if __name__ == "__main__":
    main()