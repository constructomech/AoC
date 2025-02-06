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

def count_safe_reports(filename):
    safe_reports = 0
    with open(filename, 'r') as file:
        for line in file:
            levels = list(map(int, line.strip().split()))
            if len(levels) < 2:
                continue  # Not enough levels to compare, skip this report
            if is_safe_report(levels):
                safe_reports += 1
    return safe_reports

if __name__ == "__main__":
    total_safe_reports = count_safe_reports('input.txt')
    print(f"Number of safe reports: {total_safe_reports}")
