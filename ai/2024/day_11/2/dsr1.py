from collections import defaultdict

with open('input.txt') as f:
    initial = list(map(int, f.read().split()))

current = defaultdict(int)
for num in initial:
    current[num] += 1

for _ in range(75):
    next_current = defaultdict(int)
    for num, count in current.items():
        if num == 0:
            next_current[1] += count
        else:
            s = str(num)
            if len(s) % 2 == 0:
                half = len(s) // 2
                left = int(s[:half])
                right = int(s[half:])
                next_current[left] += count
                next_current[right] += count
            else:
                new_num = num * 2024
                next_current[new_num] += count
    current = next_current

print(sum(current.values()))