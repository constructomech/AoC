import re

with open('input.txt', 'r') as f:
    data = f.read()

# Regex pattern to match do(), don't(), and mul(X,Y) instructions
pattern = r'(do\(\))|(don\'t\(\))|(mul\((\d{1,3}),(\d{1,3})\))'

current_state = True  # Initially enabled
total = 0

for match in re.finditer(pattern, data):
    if match.group(1):  # do()
        current_state = True
    elif match.group(2):  # don't()
        current_state = False
    else:  # mul(X,Y)
        x = int(match.group(4))
        y = int(match.group(5))
        if current_state:
            total += x * y

print(total)