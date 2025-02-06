# stones_optimized.py

def total_stones(n, blinks, memo):
    """
    Recursively calculates the total number of stones produced by stone n after blinks.
    Uses memoization to optimize calculations.
    """
    if blinks == 0:
        return 1  # The stone itself remains
    key = (n, blinks)
    if key in memo:
        return memo[key]

    # Apply the transformation rules
    if n == 0:
        # Rule 1: 0 becomes 1
        result = total_stones(1, blinks - 1, memo)
    elif len(str(n)) % 2 == 0:
        # Rule 2: Split the number if it has an even number of digits
        digits = str(n)
        half = len(digits) // 2
        left_digits = digits[:half].lstrip('0') or '0'
        right_digits = digits[half:].lstrip('0') or '0'
        left_num = int(left_digits)
        right_num = int(right_digits)
        result = total_stones(left_num, blinks - 1, memo) + total_stones(right_num, blinks - 1, memo)
    else:
        # Rule 3: Multiply by 2024
        new_n = n * 2024
        result = total_stones(new_n, blinks - 1, memo)

    memo[key] = result
    return result

def read_initial_stones(filename):
    """
    Reads the initial arrangement of stones from a file.
    """
    with open(filename, 'r') as file:
        line = file.readline()
        stones = [int(num) for num in line.strip().split()]
    return stones

def main():
    initial_stones = read_initial_stones('input.txt')
    blinks = 75
    memo = {}
    total = 0
    for stone in initial_stones:
        total += total_stones(stone, blinks, memo)
    print(f"After {blinks} blinks, there are {total} stones.")

if __name__ == "__main__":
    main()
