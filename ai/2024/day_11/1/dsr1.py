with open('input.txt') as f:
    initial = list(map(int, f.read().split()))

current = initial.copy()

for _ in range(25):
    next_list = []
    for n in current:
        if n == 0:
            next_list.append(1)
        else:
            s = str(n)
            if len(s) % 2 == 0:
                half = len(s) // 2
                left = int(s[:half])
                right = int(s[half:])
                next_list.append(left)
                next_list.append(right)
            else:
                next_list.append(n * 2024)
    current = next_list

print(len(current))