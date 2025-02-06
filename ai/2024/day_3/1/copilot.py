import re

def main():
    total = 0

    # Step 1: Read the corrupted memory from input.txt
    with open('input.txt', 'r') as file:
        data = file.read()

    # Step 2: Define the regex pattern for valid mul instructions
    pattern = r'mul\((\d{1,3}),(\d{1,3})\)'

    # Step 3: Find all matches in the data
    matches = re.findall(pattern, data)

    # Step 4: Compute the products and accumulate the sum
    for x_str, y_str in matches:
        x = int(x_str)
        y = int(y_str)
        product = x * y
        total += product

    # Step 5: Output the result
    print(total)

if __name__ == "__main__":
    main()
