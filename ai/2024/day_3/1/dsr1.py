import re

with open('input.txt', 'r') as f:
    data = f.read()

pattern = r'mul\((\d{1,3}),(\d{1,3})\)'
total = 0

for match in re.finditer(pattern, data):
    x = int(match.group(1))
    y = int(match.group(2))
    total += x * y

print(total)