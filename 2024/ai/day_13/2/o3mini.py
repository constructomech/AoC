#!/usr/bin/env python3
import sys

OFFSET = 10**13

def parse_machine(block):
    """
    Given a list of three lines corresponding to one machine,
    parse the moves and prize location.
    Returns:
       (a_x, a_y), (b_x, b_y), (p_x, p_y)
    where (p_x, p_y) are the original prize coordinates.
    """
    # Example lines:
    # "Button A: X+94, Y+34"
    # "Button B: X+22, Y+67"
    # "Prize: X=8400, Y=5400"
    
    # Parse Button A:
    line = block[0].strip()
    _, rest = line.split("Button A:")
    parts = rest.split(',')
    a_x = int(parts[0].strip().lstrip("X").replace("+", ""))
    a_y = int(parts[1].strip().lstrip("Y").replace("+", ""))
    
    # Parse Button B:
    line = block[1].strip()
    _, rest = line.split("Button B:")
    parts = rest.split(',')
    b_x = int(parts[0].strip().lstrip("X").replace("+", ""))
    b_y = int(parts[1].strip().lstrip("Y").replace("+", ""))
    
    # Parse Prize:
    line = block[2].strip()
    _, rest = line.split("Prize:")
    parts = rest.split(',')
    p_x = int(parts[0].strip().lstrip("X=").replace("+", ""))
    p_y = int(parts[1].strip().lstrip("Y=").replace("+", ""))
    
    return (a_x, a_y), (b_x, b_y), (p_x, p_y)

def solve_machine(a, b, p):
    """
    Given button moves a=(a_x,a_y), b=(b_x,b_y) and original prize p=(p_x,p_y),
    with the actual target being (p_x+OFFSET, p_y+OFFSET),
    solve the equations:
         a_x * A + b_x * B = (p_x+OFFSET)
         a_y * A + b_y * B = (p_y+OFFSET)
    for nonnegative integers A and B.
    
    If det != 0 then there is at most one solution:
      A = ((p_x+OFFSET)*b_y - (p_y+OFFSET)*b_x) / det,
      B = (a_x*(p_y+OFFSET) - a_y*(p_x+OFFSET)) / det,
    where det = a_x*b_y - a_y*b_x.
    
    If a valid solution exists (A and B are nonnegative integers),
    return the cost 3*A + B. Otherwise, return None.
    """
    (a_x, a_y) = a
    (b_x, b_y) = b
    (p_x, p_y) = p
    X = p_x + OFFSET
    Y = p_y + OFFSET

    det = a_x * b_y - a_y * b_x
    if det == 0:
        # In a complete solution we would handle the degenerate case,
        # but according to the puzzle the only solvable machines have det != 0.
        return None

    # Compute A and B using Cramer's rule.
    # They must be integers; otherwise, no solution exists.
    numerator_A = X * b_y - Y * b_x
    numerator_B = a_x * Y - a_y * X

    # Check that det divides the numerators exactly.
    if numerator_A % det != 0 or numerator_B % det != 0:
        return None

    A = numerator_A // det
    B = numerator_B // det

    if A < 0 or B < 0:
        return None

    return 3 * A + B

def main():
    machines = []
    try:
        with open("input.txt", "r") as f:
            content = f.read().strip()
    except Exception as e:
        print("Error reading input.txt:", e)
        sys.exit(1)
    
    # Each machine is given by 3 lines (separated by blank lines)
    blocks = content.split("\n\n")
    for block in blocks:
        lines = block.strip().splitlines()
        if len(lines) < 3:
            continue
        machines.append(parse_machine(lines))
    
    total_cost = 0
    solved_count = 0
    
    for (a, b, p) in machines:
        cost = solve_machine(a, b, p)
        if cost is not None:
            solved_count += 1
            total_cost += cost
    
    print("Prizes won:", solved_count)
    print("Fewest tokens to spend:", total_cost)

if __name__ == '__main__':
    main()
