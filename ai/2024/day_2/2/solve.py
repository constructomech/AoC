def is_safe(sequence):
    if len(sequence) < 2:
        return True
    # Check if strictly increasing
    is_increasing = True
    for i in range(len(sequence) - 1):
        if sequence[i+1] <= sequence[i]:
            is_increasing = False
            break
    # Check if strictly decreasing
    is_decreasing = True
    for i in range(len(sequence) - 1):
        if sequence[i+1] >= sequence[i]:
            is_decreasing = False
            break
    if not (is_increasing or is_decreasing):
        return False
    # Check differences
    if is_increasing:
        for i in range(len(sequence) - 1):
            diff = sequence[i+1] - sequence[i]
            if not (1 <= diff <= 3):
                return False
    else:
        for i in range(len(sequence) - 1):
            diff = sequence[i] - sequence[i+1]
            if not (1 <= diff <= 3):
                return False
    return True

def count_safe_reports_part2(reports):
    count = 0
    for line in reports:
        original = list(map(int, line.strip().split()))
        if is_safe(original):
            count += 1
            continue
        # Check all possible single removals
        safe_found = False
        for i in range(len(original)):
            modified = original[:i] + original[i+1:]
            if is_safe(modified):
                safe_found = True
                break
        if safe_found:
            count += 1
    return count

# Read the input data from input.txt
with open('input.txt', 'r') as file:
    input_data = file.read().splitlines()

# Calculate and print the result
print(count_safe_reports_part2(input_data))