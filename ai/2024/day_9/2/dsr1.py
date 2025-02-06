with open('input.txt') as f:
    line = f.readline().strip()

digits = list(line)

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

# Collect all unique file IDs present in the disk, sorted in reverse order
file_ids = sorted({x for x in disk if x != '.'}, reverse=True)

def find_leftmost_space(disk, start, required_length):
    current_run_start = -1
    current_run_length = 0
    for i in range(start):
        if disk[i] == '.':
            if current_run_start == -1:
                current_run_start = i
            current_run_length += 1
            if current_run_length >= required_length:
                return current_run_start
        else:
            current_run_start = -1
            current_run_length = 0
    # Check if there's a valid run after the loop
    if current_run_length >= required_length:
        return current_run_start
    return -1

for fid in file_ids:
    # Find the current start and end indices of the file
    start = None
    end = None
    for i in range(len(disk)):
        if disk[i] == fid:
            if start is None:
                start = i
            end = i
    if start is None:
        continue  # This file is not present (should not happen according to problem statement)
    length = end - start + 1
    
    # Find leftmost free space to the left of the current start
    new_start = find_leftmost_space(disk, start, length)
    if new_start != -1:
        # Replace the original positions with free space
        for i in range(start, end + 1):
            disk[i] = '.'
        # Place the file in the new position
        for i in range(new_start, new_start + length):
            disk[i] = fid

# Calculate the checksum
checksum = 0
for idx, block in enumerate(disk):
    if block != '.':
        checksum += idx * block

print(checksum)