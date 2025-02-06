import re

def main():
    # Step 1: Read the corrupted memory from input.txt
    with open('input.txt', 'r') as file:
        data = file.read()

    # Step 2: Define regex patterns for valid instructions
    # Pattern for valid mul(X,Y) instructions
    pattern_mul = re.compile(r'mul\((\d{1,3}),(\d{1,3})\)')
    # Pattern for control instructions: do(), don't(), undo()
    pattern_control = re.compile(r"(do|don't|undo)\(\)")

    # Step 3: Find all matches along with their positions
    matches = []

    # Find mul instructions
    for match in pattern_mul.finditer(data):
        matches.append((match.start(), 'mul', match))

    # Find control instructions
    for match in pattern_control.finditer(data):
        control_cmd = match.group(1)
        if control_cmd == 'do' or control_cmd == 'undo':
            matches.append((match.start(), 'do', match))
        elif control_cmd == "don't":  # Changed to double quotes here
            matches.append((match.start(), "don't", match))

    # Step 4: Sort the matches based on their position in the string
    matches.sort(key=lambda x: x[0])

    # Step 5: Process instructions while maintaining state
    total = 0
    mul_enabled = True  # 'mul' instructions are enabled at the beginning

    for _, instr_type, match_obj in matches:
        if instr_type == 'do':
            mul_enabled = True
        elif instr_type == "don't":
            mul_enabled = False
        elif instr_type == 'mul':
            if mul_enabled:
                x_str, y_str = match_obj.groups()
                x = int(x_str)
                y = int(y_str)
                product = x * y
                total += product

    # Step 7: Output the total sum
    print(total)

if __name__ == "__main__":
    main()
