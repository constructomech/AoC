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

# Simulate the compaction process
while True:
    # Find the leftmost free space
    try:
        left = disk.index('.')
    except ValueError:
        break  # No more free spaces
    
    # Find the rightmost non-free space
    right = len(disk) - 1
    while right >= 0 and disk[right] == '.':
        right -= 1
    
    if right < left:
        break  # No more blocks to move
    
    # Move the block
    disk[left] = disk[right]
    disk[right] = '.'

# Calculate the checksum
checksum = 0
for idx, block in enumerate(disk):
    if block != '.':
        checksum += idx * block

print(checksum)