import itertools

def calculate_calibration_sum(file_path):
    sum_total = 0
    with open(file_path, 'r') as file:
        for line in file:
            line = line.strip()
            if not line:
                continue
            parts = line.split(':')
            test_value = int(parts[0].strip())
            numbers = list(map(int, parts[1].strip().split()))
            if len(numbers) == 0:
                continue  # invalid line
            if len(numbers) == 1:
                if numbers[0] == test_value:
                    sum_total += test_value
                continue
            # Generate all possible operator combinations including concatenation
            num_ops = len(numbers) - 1
            valid = False
            for ops in itertools.product(['+', '*', '||'], repeat=num_ops):
                current = numbers[0]
                for i in range(num_ops):
                    op = ops[i]
                    next_num = numbers[i + 1]
                    if op == '+':
                        current += next_num
                    elif op == '*':
                        current *= next_num
                    elif op == '||':
                        # Concatenate current and next_num as strings, then convert back to integer
                        current = int(str(current) + str(next_num))
                if current == test_value:
                    valid = True
                    break  # No need to check other combinations once a valid one is found
            if valid:
                sum_total += test_value
    return sum_total

# Example usage
print(calculate_calibration_sum('input.txt'))