#!/usr/bin/env python3

def parse_machine(block):
    """
    Given a list of three lines corresponding to one machine,
    parse the moves and prize location.
    Returns:
       (a_x, a_y), (b_x, b_y), (p_x, p_y)
    """
    # Example lines:
    # "Button A: X+94, Y+34"
    # "Button B: X+22, Y+67"
    # "Prize: X=8400, Y=5400"
    
    # Parse Button A:
    line = block[0].strip()
    # Remove "Button A:" prefix
    _, rest = line.split("Button A:")
    # Now rest should be " X+94, Y+34"
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
    Given button moves a=(a_x,a_y), b=(b_x,b_y) and prize p=(p_x,p_y),
    find nonnegative integers A and B (with 0<=A,B<=100) such that:
         A*a_x + B*b_x = p_x
         A*a_y + B*b_y = p_y
    and return the minimum cost 3*A + B. If no solution exists, return None.
    """
    (a_x, a_y) = a
    (b_x, b_y) = b
    (p_x, p_y) = p
    min_cost = None
    
    for A in range(101):
        for B in range(101):
            if A * a_x + B * b_x == p_x and A * a_y + B * b_y == p_y:
                cost = 3 * A + B
                if min_cost is None or cost < min_cost:
                    min_cost = cost
    return min_cost

def main():
    machines = []
    with open("input.txt", "r") as f:
        # Split input into blocks separated by blank lines
        content = f.read().strip()
    # Each machine is represented by 3 lines; blocks are separated by blank lines.
    blocks = content.split("\n\n")
    for block in blocks:
        lines = block.strip().splitlines()
        if len(lines) < 3:
            continue  # skip any incomplete blocks
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
