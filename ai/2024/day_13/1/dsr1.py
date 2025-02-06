import re

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
    target_x = int(match.group(1))
    target_y = int(match.group(2))
    
    min_cost = None
    for a in range(0, 101):
        for b in range(0, 101):
            x = a * dx_a + b * dx_b
            y = a * dy_a + b * dy_b
            if x == target_x and y == target_y:
                cost = 3 * a + b
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