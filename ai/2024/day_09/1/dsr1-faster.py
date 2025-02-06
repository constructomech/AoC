# Read input
with open('input.txt') as f:
    line = f.readline().strip()

digits = list(line)

# Parse the disk
disk = []
file_id = 0
for i in range(0, len(digits), 2):
    file_len = int(digits[i])
    if file_len > 0:
        disk.extend([file_id] * file_len)
    if i + 1 < len(digits):
        free_len = int(digits[i + 1])
        disk.extend(['.'] * free_len)
    file_id += 1

# Optimized compaction using two-pointer approach
left = 0
right = len(disk) - 1

while left < right:
    # Find next free space from the left
    while left < right and disk[left] != '.':
        left += 1
    # Find next non-free space from the right
    while left < right and disk[right] == '.':
        right -= 1
    if left < right:
        # Swap the block
        disk[left], disk[right] = disk[right], disk[left]
        left += 1
        right -= 1

# Calculate the checksum
checksum = 0
for idx, block in enumerate(disk):
    if block != '.':
        checksum += idx * block

print(checksum)