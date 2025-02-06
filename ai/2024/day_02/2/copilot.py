def is_safe_report(levels):
    # Compute differences between adjacent levels
    differences = [levels[i+1] - levels[i] for i in range(len(levels) - 1)]
    
    # Check for zero differences (levels are equal)
    if any(diff == 0 for diff in differences):
        return False  # Adjacent levels should not be equal

    # Check that all differences are positive (increasing) or all negative (decreasing)
    is_increasing = all(diff > 0 for diff in differences)
    is_decreasing = all(diff < 0 for diff in differences)
    
    if not (is_increasing or is_decreasing):
        return False  # Levels are not all increasing or all decreasing

    # Check that all differences have absolute value between 1 and 3 inclusive
    if all(1 <= abs(diff) <= 3 for diff in differences):
        return True  # The report is safe
    else:
        return False  # Differences are out of the allowed range

def is_safe_with_dampener(levels):
    # First, check if the original report is safe
    if is_safe_report(levels):
        return True

    # Try removing each level one at a time
    for i in range(len(levels)):
        modified_levels = levels[:i] + levels[i+1:]
        if len(modified_levels) < 2:
            continue  # Not enough levels to compare
        if is_safe_report(modified_levels):
            return True  # Found a safe report by removing one level
    return False  # None of the modified reports are safe

def count_safe_reports_with_dampener(filename):
    safe_reports = 0
    with open(filename, 'r') as file:
        for line in file:
            levels = list(map(int, line.strip().split()))
            if len(levels) < 2:
                continue  # Not enough levels to compare
            if is_safe_with_dampener(levels):
                safe_reports += 1
    return safe_reports

if __name__ == "__main__":
    total_safe_reports = count_safe_reports_with_dampener('input.txt')
    print(f"Number of safe reports with Problem Dampener: {total_safe_reports}")
