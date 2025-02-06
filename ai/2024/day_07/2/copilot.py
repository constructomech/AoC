def main():
    total_calibration_result = 0
    with open('input.txt', 'r') as f:
        lines = f.readlines()

    for line_num, line in enumerate(lines, start=1):
        line = line.strip()
        if not line or line.startswith('#'):
            continue  # Skip empty lines and comments
        if ':' not in line:
            print(f"Skipping invalid format on line {line_num}: {line}")
            continue  # Skip lines without a colon

        # Parse the test value and the numbers
        test_value_str, numbers_str = line.split(':', 1)
        try:
            test_value = int(test_value_str.strip())
            numbers = list(map(int, numbers_str.strip().split()))
        except ValueError:
            print(f"Invalid numbers on line {line_num}: {line}")
            continue  # Skip lines with invalid integers

        # Check if the equation can be made true
        if can_evaluate_to_value(test_value, numbers):
            total_calibration_result += test_value

    print(total_calibration_result)

def can_evaluate_to_value(test_value, numbers):
    N = len(numbers)
    if N == 0:
        return False  # No numbers to evaluate

    # When there's only one number, check if it equals the test value
    if N == 1:
        return numbers[0] == test_value

    # Generate all possible combinations of '+', '*', '||'
    num_operators = N - 1
    operator_options = ['+', '*', '||']
    from itertools import product
    operator_combinations = product(operator_options, repeat=num_operators)

    for ops in operator_combinations:
        result = evaluate_expression(numbers, ops)
        if result == test_value:
            return True  # Found a valid combination

    return False  # No combination matched the test value

def evaluate_expression(numbers, operators):
    total = numbers[0]
    for op, num in zip(operators, numbers[1:]):
        if op == '+':
            total += num
        elif op == '*':
            total *= num
        elif op == '||':
            total = int(str(total) + str(num))
        else:
            # Should not reach here
            return None
    return total

if __name__ == '__main__':
    main()
