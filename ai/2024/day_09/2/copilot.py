def main():
    # Read the disk map from input.txt
    with open('input.txt', 'r') as f:
        disk_map_str = f.read().strip()

    # Parse the disk map to get lengths of files and free spaces
    lengths = list(map(int, disk_map_str))

    disk = []
    file_id = 0
    is_file = True  # Start with a file
    file_positions = []  # List of (file_id, start_index, end_index)
    free_spaces = []     # List of (start_index, end_index)

    index = 0  # Current index on the disk

    # Build the initial disk representation and record positions
    for length in lengths:
        if is_file:
            # Append file blocks with current file_id
            disk.extend([file_id] * length)
            file_positions.append({'file_id': file_id, 'start': index, 'end': index + length - 1})
            index += length
            file_id += 1
        else:
            # Append free space (None)
            disk.extend([None] * length)
            free_spaces.append({'start': index, 'end': index + length - 1, 'length': length})
            index += length
        is_file = not is_file  # Toggle between file and free space

    # --------------------
    # Compaction Process
    # --------------------

    # Sort files in decreasing order of file_id
    file_positions.sort(key=lambda x: x['file_id'], reverse=True)

    for file in file_positions:
        file_id = file['file_id']
        file_start = file['start']
        file_end = file['end']
        file_length = file_end - file_start + 1

        # Find all free spaces to the left of the file
        suitable_spaces = [space for space in free_spaces if space['end'] < file_start and space['length'] >= file_length]

        if suitable_spaces:
            # Choose the leftmost suitable space
            target_space = min(suitable_spaces, key=lambda x: x['start'])

            # Move the file to the new position
            new_start = target_space['start']
            new_end = new_start + file_length - 1

            # Update the disk representation
            # Clear old file positions
            for i in range(file_start, file_end + 1):
                disk[i] = None

            # Place the file in the new position
            for i in range(new_start, new_end + 1):
                disk[i] = file_id

            # Update the file's recorded position
            file['start'] = new_start
            file['end'] = new_end

            # Update the free spaces
            # Remove the used portion of the free space
            if target_space['length'] == file_length:
                # Entire free space is used; remove it
                free_spaces.remove(target_space)
            else:
                # Adjust the free space to account for the used portion
                remaining_length = target_space['length'] - file_length
                new_space_start = target_space['start'] + file_length
                new_space_end = target_space['end']
                free_spaces.remove(target_space)
                free_spaces.append({'start': new_space_start, 'end': new_space_end, 'length': remaining_length})

            # Create a new free space where the file was originally, if any
            old_space_start = file_start
            old_space_end = file_end
            old_space_length = file_length

            # Insert the new free space into free_spaces list
            free_spaces.append({'start': old_space_start, 'end': old_space_end, 'length': old_space_length})
        else:
            # No suitable free space; file remains in place
            continue

    # It's important to re-sort the free_spaces list if we need to find leftmost spaces again
    # However, since we process files in decreasing order and only move files to the left, existing free spaces are sufficient

    # --------------------
    # Calculate the Checksum
    # --------------------
    checksum = sum(idx * file_id for idx, file_id in enumerate(disk) if file_id is not None)

    # Output the checksum
    print(f"The resulting filesystem checksum is: {checksum}")

if __name__ == "__main__":
    main()
