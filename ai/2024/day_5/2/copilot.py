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
            if line:  # Ensure line is not empty
                update = list(map(int, line.split(',')))
                updates.append(update)

    return ordering_rules, updates

# Validate updates and compute the sum of middle pages
def process_updates(ordering_rules, updates):
    valid_updates = []
    invalid_updates = []

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
            valid_updates.append(update)
        else:
            invalid_updates.append(update)

    return valid_updates, invalid_updates

# Perform topological sort
def topological_sort(pages, ordering_rules):
    from collections import defaultdict, deque

    # Build the graph
    graph = defaultdict(list)
    in_degree = defaultdict(int)

    # Initialize in-degree for all pages
    for page in pages:
        in_degree[page] = 0

    # Add edges and compute in-degrees
    for x, y in ordering_rules:
        if x in pages and y in pages:
            graph[x].append(y)
            in_degree[y] += 1

    # Queue for nodes with no incoming edges
    queue = deque([page for page in pages if in_degree[page] == 0])

    sorted_pages = []
    while queue:
        node = queue.popleft()
        sorted_pages.append(node)
        for neighbor in graph[node]:
            in_degree[neighbor] -= 1
            if in_degree[neighbor] == 0:
                queue.append(neighbor)

    if len(sorted_pages) != len(pages):
        # Cycle detected; cannot perform topological sort
        return None

    return sorted_pages

def process_invalid_updates(ordering_rules, invalid_updates):
    total_middle_page_sum = 0

    for update in invalid_updates:
        # Extract relevant ordering rules
        relevant_rules = [(x, y) for x, y in ordering_rules if x in update and y in update]

        # Perform topological sort
        sorted_update = topological_sort(update, relevant_rules)

        if sorted_update is None:
            print(f"Cannot reorder update {update} due to cycle in ordering rules.")
            continue  # Skip this update

        # Compute middle page
        middle_index = len(sorted_update) // 2
        middle_page = sorted_update[middle_index]
        total_middle_page_sum += middle_page

    return total_middle_page_sum

def main():
    ordering_rules, updates = read_input('input.txt')
    _, invalid_updates = process_updates(ordering_rules, updates)
    result = process_invalid_updates(ordering_rules, invalid_updates)
    print(result)

if __name__ == '__main__':
    main()
