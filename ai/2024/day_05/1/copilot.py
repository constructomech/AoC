# Read the input file
def read_input(filename):
    with open(filename, 'r') as file:
        lines = file.readlines()

    # Remove any trailing newlines and spaces
    lines = [line.strip() for line in lines]

    # Separate ordering rules and updates
    ordering_rules = []
    updates = []
    separator_found = False

    for line in lines:
        if line == '':
            separator_found = True
            continue
        if not separator_found:
            # Parse ordering rules
            if '|' in line:
                x, y = map(int, line.split('|'))
                ordering_rules.append((x, y))
        else:
            # Parse updates
            update = list(map(int, line.split(',')))
            updates.append(update)

    return ordering_rules, updates

# Validate updates and compute the sum of middle pages
def process_updates(ordering_rules, updates):
    total_middle_page_sum = 0

    for update in updates:
        is_valid = True
        page_indices = {page: idx for idx, page in enumerate(update)}

        for x, y in ordering_rules:
            if x in page_indices and y in page_indices:
                if page_indices[x] >= page_indices[y]:
                    # Ordering rule violated
                    is_valid = False
                    break

        if is_valid:
            # Find middle page
            middle_index = len(update) // 2
            middle_page = update[middle_index]
            total_middle_page_sum += middle_page

    return total_middle_page_sum

def main():
    ordering_rules, updates = read_input('input.txt')
    result = process_updates(ordering_rules, updates)
    print(result)

if __name__ == '__main__':
    main()
