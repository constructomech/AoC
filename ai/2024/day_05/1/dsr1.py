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

sum_middle = 0

for update in updates:
    pages_list = update
    pages_set = set(pages_list)
    pos = {page: idx for idx, page in enumerate(pages_list)}
    valid = True
    for x, y in rules:
        if x in pages_set and y in pages_set:
            if pos[x] >= pos[y]:
                valid = False
                break
    if valid:
        n = len(pages_list)
        middle_index = (n - 1) // 2
        sum_middle += pages_list[middle_index]

print(sum_middle)