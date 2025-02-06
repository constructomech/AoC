def calculate_total_distance():
    left_list = []
    right_list = []

    # Read the input file and parse the numbers
    with open('input.txt', 'r') as file:
        for line in file:
            if line.strip():  # Skipping any empty lines
                left_str, right_str = line.strip().split()
                left_num = int(left_str)
                right_num = int(right_str)
                left_list.append(left_num)
                right_list.append(right_num)

    # Sort both lists
    left_list.sort()
    right_list.sort()

    # Pair up the numbers and calculate the total distance
    total_distance = sum(abs(l - r) for l, r in zip(left_list, right_list))

    print("Total distance between the lists:", total_distance)

# Run the function
calculate_total_distance()
