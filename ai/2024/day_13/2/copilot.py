def parse_input(input_file):
    machines = []
    with open(input_file, 'r') as f:
        lines = f.readlines()

    i = 0
    while i < len(lines):
        line = lines[i].strip()
        if line.startswith('Button A:'):
            dx1, dy1 = parse_button_line(line)
            i += 1
            dx2, dy2 = parse_button_line(lines[i].strip())
            i += 1
            X_p, Y_p = parse_prize_line(lines[i].strip())
            i += 1
            machines.append({
                'dx1': dx1, 'dy1': dy1,
                'dx2': dx2, 'dy2': dy2,
                'X_p': X_p, 'Y_p': Y_p
            })
        else:
            i += 1
    return machines

def parse_button_line(line):
    parts = line.split(':')[1].split(',')
    dx_part = parts[0].strip()
    dy_part = parts[1].strip()
    dx = int(dx_part.split('+')[1])
    dy = int(dy_part.split('+')[1])
    return dx, dy

def parse_prize_line(line):
    parts = line.split(':')[1].split(',')
    X_p_part = parts[0].strip()
    Y_p_part = parts[1].strip()
    X_p = int(X_p_part.split('=')[1])
    Y_p = int(Y_p_part.split('=')[1])
    return X_p, Y_p

def find_min_tokens(machine):
    from math import gcd

    dx1 = machine['dx1']
    dy1 = machine['dy1']
    dx2 = machine['dx2']
    dy2 = machine['dy2']
    X_p = machine['X_p']
    Y_p = machine['Y_p']

    # Determinant
    D = dx1 * dy2 - dx2 * dy1

    if D == 0:
        if dx1 * Y_p == dy1 * X_p:
            return None
        else:
            return None

    # Compute particular solutions
    rhs_nA = dx2 * Y_p - dy2 * X_p
    rhs_nB = dx1 * Y_p - dy1 * X_p

    if rhs_nA % D != 0 or rhs_nB % (-D) != 0:
        return None  # No integer solution

    n_A0 = rhs_nA // D
    n_B0 = rhs_nB // (-D)

    # Parameterization coefficients
    delta_nA = dx2 // abs(gcd(dx2, D))
    delta_nB = -dx1 // abs(gcd(dx1, D))

    # Find minimal non-negative integer solutions
    t_candidates = []

    # Compute t bounds to ensure n_A and n_B are non-negative
    t_low_nA = (-n_A0 * D) // dx2 if dx2 != 0 else 0
    t_low_nB = (-n_B0 * D) // (-dx1) if dx1 != 0 else 0

    t_start = max(t_low_nA, t_low_nB)
    t_end = t_start + 1000  # Adjust range as needed

    min_total_tokens = None

    for t in range(int(t_start), int(t_end)):
        n_A_candidate = n_A0 + (dx2 * t) // D
        n_B_candidate = n_B0 + (-dx1 * t) // D

        if n_A_candidate >= 0 and n_B_candidate >= 0:
            total_tokens = 3 * n_A_candidate + n_B_candidate
            if min_total_tokens is None or total_tokens < min_total_tokens:
                min_total_tokens = total_tokens

    return min_total_tokens

def main():
    machines = parse_input('input.txt')
    total_tokens = 0
    solvable_machines = 0
    for idx, machine in enumerate(machines):
        min_tokens = find_min_tokens(machine)
        if min_tokens is not None:
            total_tokens += min_tokens
            solvable_machines += 1
            print(f"Machine {idx+1}: Minimum tokens required = {min_tokens}")
        else:
            print(f"Machine {idx+1}: Cannot win the prize.")
    print(f"\nThe fewest tokens required to win all possible prizes is: {total_tokens}")

if __name__ == '__main__':
    main()
