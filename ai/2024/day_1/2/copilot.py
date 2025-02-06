from collections import Counter

def calculate_similarity_score():
    left_list = []
    right_list = []

    # Read the input file and parse the numbers
    with open('input.txt', 'r') as file:
        for line_number, line in enumerate(file, 1):
            if line.strip():  # Skipping any empty lines
                try:
                    left_str, right_str = line.strip().split()
                    left_num = int(left_str)
                    right_num = int(right_str)
                    left_list.append(left_num)
                    right_list.append(right_num)
                except ValueError:
                    print(f"Warning: Skipping invalid line {line_number}: {line.strip()}")
                    continue

    # Count the frequency of each number in the right list
    right_counter = Counter(right_list)

    # Calculate the similarity score
    similarity_score = 0
    for num in left_list:
        frequency = right_counter.get(num, 0)
        contribution = num * frequency
        similarity_score += contribution
        # Uncomment the next line to see detailed contributions
        # print(f"Number: {num}, Frequency in right list: {frequency}, Contribution: {contribution}")

    print("Similarity score:", similarity_score)

# Run the function
if __name__ == "__main__":
    calculate_similarity_score()
