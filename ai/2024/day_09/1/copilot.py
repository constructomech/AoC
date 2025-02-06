def main():
    # Read the disk map from input.txt
    with open('input.txt', 'r') as f:
        disk_map_str = f.read().strip()

    # Parse the disk map to get lengths of files and free spaces
    lengths = list(map(int, disk_map_str))

    disk = []
    file_id = 0
    is_file = True  # Start with a file

    # Build the initial disk representation
    for length in lengths:
        if is_file:
            # Append file blocks with current file_id
            disk.extend([file_id] * length)
            file_id += 1
        else:
            # Append free space (None)
            disk.extend([None] * length)
        is_file = not is_file  # Toggle between file and free space

    # Compact the disk using two pointers
    left_ptr = 0
    right_ptr = len(disk) - 1

    while left_ptr < right_ptr:
        # Move left_ptr to the next free space
        while left_ptr < right_ptr and disk[left_ptr] is not None:
            left_ptr += 1
        # Move right_ptr to the next file block that can be moved
        while left_ptr < right_ptr and disk[right_ptr] is None:
            right_ptr -= 1
        if left_ptr < right_ptr:
            # Move the block from right_ptr to left_ptr
            disk[left_ptr] = disk[right_ptr]
            disk[right_ptr] = None
            left_ptr += 1
            right_ptr -= 1

    # Calculate the checksum
    checksum = sum(idx * file_id for idx, file_id in enumerate(disk) if file_id is not None)

    # Output the checksum
    print(f"The resulting filesystem checksum is: {checksum}")

if __name__ == "__main__":
    main()
