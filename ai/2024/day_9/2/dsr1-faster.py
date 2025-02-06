import bisect

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

# Precompute file positions
file_positions = {}
current_start = None
current_id = None
for i, block in enumerate(disk):
    if block != '.':
        if current_id != block:
            current_id = block
            current_start = i
            if current_id not in file_positions:
                file_positions[current_id] = {'start': i, 'end': i}
            else:
                pass  # Contiguity ensures this shouldn't happen
        file_positions[current_id]['end'] = i
    else:
        current_id = None
        current_start = None

# Precompute free intervals
free_intervals = []
current_start = None
current_length = 0
for i, block in enumerate(disk):
    if block == '.':
        if current_start is None:
            current_start = i
            current_length = 1
        else:
            current_length += 1
    else:
        if current_start is not None:
            free_intervals.append((current_start, current_length))
            current_start = None
            current_length = 0
if current_start is not None:
    free_intervals.append((current_start, current_length))
free_intervals.sort()

# Process each file in reverse order
for fid in sorted(file_positions.keys(), reverse=True):
    pos = file_positions[fid]
    start = pos['start']
    end = pos['end']
    length = end - start + 1

    # Find the earliest applicable free interval
    best_candidate = None
    for interval in free_intervals:
        interval_start, interval_length = interval
        if interval_start + interval_length > start:
            continue
        if interval_length >= length:
            best_candidate = interval
            break  # earliest possible

    if not best_candidate:
        continue

    new_start = best_candidate[0]
    new_end = new_start + length - 1

    # Update file positions
    file_positions[fid]['start'] = new_start
    file_positions[fid]['end'] = new_end

    # Remove the best_candidate from free_intervals
    free_intervals.remove(best_candidate)

    # Add remaining part of the best_candidate if any
    remaining_length = best_candidate[1] - length
    if remaining_length > 0:
        remaining_start = new_start + length
        remaining_interval = (remaining_start, remaining_length)
        insert_pos = bisect.bisect_left(free_intervals, (remaining_start,))
        free_intervals.insert(insert_pos, remaining_interval)

    # Add original file's space as new free interval
    original_free_start = start
    original_free_length = length
    new_free_interval = (original_free_start, original_free_length)

    # Merge with adjacent intervals
    insert_pos = bisect.bisect_left(free_intervals, new_free_interval)

    # Check previous interval
    if insert_pos > 0:
        prev = free_intervals[insert_pos - 1]
        if prev[0] + prev[1] == new_free_interval[0]:
            new_free_interval = (prev[0], prev[1] + new_free_interval[1])
            del free_intervals[insert_pos - 1]
            insert_pos -= 1

    # Check next interval
    if insert_pos < len(free_intervals):
        next_int = free_intervals[insert_pos]
        if new_free_interval[0] + new_free_interval[1] == next_int[0]:
            new_free_interval = (new_free_interval[0], new_free_interval[1] + next_int[1])
            del free_intervals[insert_pos]

    # Insert the merged interval
    free_intervals.insert(insert_pos, new_free_interval)

# Compute checksum using arithmetic series sum
checksum = 0
for fid in file_positions:
    pos = file_positions[fid]
    start = pos['start']
    end = pos['end']
    count = end - start + 1
    total = (start + end) * count // 2
    checksum += fid * total

print(checksum)