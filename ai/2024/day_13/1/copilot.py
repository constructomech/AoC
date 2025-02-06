# claw_machines.py

import re

def parse_input(file_name):
    machines = []
    with open(file_name, 'r') as file:
        lines = file.readlines()

    i = 0
    while i < len(lines):
        line = lines[i].strip()
        if not line:
            i += 1
            continue

        # Parse Button A
        match_a = re.match(r'Button A: X\+(\d+), Y\+(\d+)', line)
        delta_x_a = int(match_a.group(1))
        delta_y_a = int(match_a.group(2))
        i += 1

        # Parse Button B
        line = lines[i].strip()
        match_b = re.match(r'Button B: X\+(\d+), Y\+(\d+)', line)
        delta_x_b = int(match_b.group(1))
        delta_y_b = int(match_b.group(2))
        i += 1

        # Parse Prize position
        line = lines[i].strip()
        match_p = re.match(r'Prize: X=(\d+), Y=(\d+)', line)
        prize_x = int(match_p.group(1))
        prize_y = int(match_p.group(2))
        i += 1

        # Append the machine's data
        machine = {
            'delta_x_a': delta_x_a,
            'delta_y_a': delta_y_a,
            'delta_x_b': delta_x_b,
            'delta_y_b': delta_y_b,
            'prize_x': prize_x,
            'prize_y': prize_y
        }
        machines.append(machine)

    return machines

def solve_machine(machine):
    delta_x_a = machine['delta_x_a']
    delta_y_a = machine['delta_y_a']
    delta_x_b = machine['delta_x_b']
    delta_y_b = machine['delta_y_b']
    prize_x = machine['prize_x']
    prize_y = machine['prize_y']

    min_cost = None
    optimal_a = None
    optimal_b = None

    # Since we can press each button up to 100 times
    max_presses = 100

    # Iterate over possible values of 'a' and 'b'
    for a in range(0, max_presses + 1):
        for b in range(0, max_presses + 1):
            total_x = a * delta_x_a + b * delta_x_b
            total_y = a * delta_y_a + b * delta_y_b

            if total_x == prize_x and total_y == prize_y:
                cost = 3 * a + b
                if min_cost is None or cost < min_cost:
                    min_cost = cost
                    optimal_a = a
                    optimal_b = b

    if min_cost is not None:
        return {
            'min_cost': min_cost,
            'a': optimal_a,
            'b': optimal_b
        }
    else:
        return None

def main():
    machines = parse_input('input.txt')
    total_tokens = 0
    prizes_won = 0

    for idx, machine in enumerate(machines):
        result = solve_machine(machine)
        if result:
            prizes_won += 1
            total_tokens += result['min_cost']
            print(f"Machine {idx + 1}: Won prize with {result['a']} presses of A and {result['b']} presses of B, costing {result['min_cost']} tokens.")
        else:
            print(f"Machine {idx + 1}: Cannot win prize.")

    print(f"\nTotal prizes won: {prizes_won}")
    print(f"Minimum total tokens required: {total_tokens}")

if __name__ == "__main__":
    main()
