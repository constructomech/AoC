with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

# Split the input into rules and updates
try:
    split_index = lines.index('')
except ValueError:
    split_index = len(lines)
rules_lines = lines[:split_index]
updates_lines = lines[split_index+1:]

# Parse the rules into a list of tuples (x, y)
rules = []
for line in rules_lines:
    x, y = map(int, line.split('|'))
    rules.append((x, y))

# Parse the updates into lists of integers
updates = []
for line in updates_lines:
    if not line:
        continue
    pages = list(map(int, line.split(',')))
    updates.append(pages)

# Separate valid and invalid updates
valid_updates = []
invalid_updates = []

for update in updates:
    pages_set = set(update)
    pos = {page: idx for idx, page in enumerate(update)}
    valid = True
    for x, y in rules:
        if x in pages_set and y in pages_set:
            if pos[x] >= pos[y]:
                valid = False
                break
    if valid:
        valid_updates.append(update)
    else:
        invalid_updates.append(update)

def correct_order(update, all_rules):
    pages = update
    page_set = set(pages)
    applicable_rules = [(x, y) for x, y in all_rules if x in page_set and y in page_set]
    
    adj = {}
    in_degree = {}
    original_pos = {page: idx for idx, page in enumerate(update)}
    for page in pages:
        adj[page] = []
        in_degree[page] = 0
    
    for x, y in applicable_rules:
        adj[x].append(y)
        in_degree[y] += 1
    
    queue = [page for page in pages if in_degree[page] == 0]
    queue.sort(key=lambda p: original_pos[p])
    result = []
    
    while queue:
        node = queue.pop(0)
        result.append(node)
        for neighbor in adj[node]:
            in_degree[neighbor] -= 1
            if in_degree[neighbor] == 0:
                queue.append(neighbor)
        queue.sort(key=lambda p: original_pos[p])
    
    if len(result) != len(pages):
        return None
    return result

sum_corrected = 0

for update in invalid_updates:
    corrected = correct_order(update, rules)
    if corrected:
        n = len(corrected)
        middle_index = (n - 1) // 2
        sum_corrected += corrected[middle_index]

print(sum_corrected)