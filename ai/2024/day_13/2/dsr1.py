import re
import math

def parse_input(filename):
    with open(filename, 'r') as file:
        lines = [line.strip() for line in file.readlines()]
    
    machines = []
    current_machine = []
    for line in lines:
        if line:
            current_machine.append(line)
        else:
            if current_machine:
                machines.append(current_machine)
                current_machine = []
    if current_machine:
        machines.append(current_machine)
    return machines

def extended_gcd(a, b):
    if b == 0:
        return (a, 1, 0)
    else:
        g, x, y = extended_gcd(b, a % b)
        return (g, y, x - (a // b) * y)

def process_machine(machine_lines):
    a_line = machine_lines[0]
    b_line = machine_lines[1]
    prize_line = machine_lines[2]
    
    # Parse A button
    match = re.search(r"X\+(\d+), Y\+(\d+)", a_line)
    dx_a = int(match.group(1))
    dy_a = int(match.group(2))
    
    # Parse B button
    match = re.search(r"X\+(\d+), Y\+(\d+)", b_line)
    dx_b = int(match.group(1))
    dy_b = int(match.group(2))
    
    # Parse prize
    match = re.search(r"X=(\d+), Y=(\d+)", prize_line)
    target_x = int(match.group(1)) + 10**13
    target_y = int(match.group(2)) + 10**13
    
    D = dx_a * dy_b - dx_b * dy_a
    if D != 0:
        a_numerator = target_x * dy_b - target_y * dx_b
        b_numerator = target_y * dx_a - target_x * dy_a
        if a_numerator % D != 0 or b_numerator % D != 0:
            return None
        a = a_numerator // D
        b = b_numerator // D
        return 3 * a + b if a >= 0 and b >= 0 else None
    else:
        if (target_x * dy_b != target_y * dx_b) or (target_x * dy_a != target_y * dx_a):
            return None
        g = math.gcd(dx_a, dx_b)
        if target_x % g != 0:
            return None
        X_prime = target_x // g
        dx_a_prime, dx_b_prime = dx_a // g, dx_b // g
        g_extended, x, y = extended_gcd(dx_a_prime, dx_b_prime)
        if g_extended != 1:
            return None
        a0 = x * X_prime
        b0 = y * X_prime
        t_low = math.ceil((-a0) / dx_b_prime)
        t_high = math.floor(b0 / dx_a_prime)
        if t_low > t_high:
            return None
        min_cost = None
        for t in [t_low, t_high]:
            a = a0 + dx_b_prime * t
            b_val = b0 - dx_a_prime * t
            if a >= 0 and b_val >= 0:
                cost = 3 * a + b_val
                if min_cost is None or cost < min_cost:
                    min_cost = cost
        return min_cost

def main():
    machines = parse_input('input.txt')
    total_cost = 0
    for machine in machines:
        cost = process_machine(machine)
        if cost is not None:
            total_cost += cost
    print(total_cost)

if __name__ == "__main__":
    main()