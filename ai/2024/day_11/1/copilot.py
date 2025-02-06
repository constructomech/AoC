# stones_simulation.py

def transform_stone(n):
    """
    Transforms a stone according to the specified rules.
    """
    if n == 0:
        # Rule 1: 0 becomes 1
        return [1]
    elif len(str(n)) % 2 == 0:
        # Rule 2: Split the number if it has an even number of digits
        digits = str(n)
        half = len(digits) // 2
        left_digits = digits[:half]
        right_digits = digits[half:]
        left_num = int(left_digits)  # No extra leading zeros
        right_num = int(right_digits)
        return [left_num, right_num]
    else:
        # Rule 3: Multiply by 2024
        new_n = n * 2024
        return [new_n]

def read_initial_stones(filename):
    """
    Reads the initial arrangement of stones from a file.
    """
    with open(filename, 'r') as file:
        line = file.readline()
        stones = [int(num) for num in line.strip().split()]
    return stones

def simulate_blinks(stones, blinks):
    """
    Simulates the stones' transformations over a given number of blinks.
    """
    for blink in range(blinks):
        new_stones = []
        for stone in stones:
            transformed = transform_stone(stone)
            new_stones.extend(transformed)
        stones = new_stones
    return stones

def main():
    initial_stones = read_initial_stones('input.txt')
    blinks = 25
    final_stones = simulate_blinks(initial_stones, blinks)
    print(f"After {blinks} blinks, there are {len(final_stones)} stones.")

if __name__ == "__main__":
    main()
