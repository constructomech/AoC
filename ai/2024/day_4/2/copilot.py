def count_x_mas_occurrences(grid):
    rows = len(grid)
    cols = len(grid[0])
    words = ["MAS", "SAM"]
    total_count = 0

    # Iterate over all possible center positions
    for x in range(1, rows - 1):
        for y in range(1, cols - 1):
            # Extract letters from both diagonals
            diag1 = grid[x - 1][y - 1] + grid[x][y] + grid[x + 1][y + 1]
            diag2 = grid[x - 1][y + 1] + grid[x][y] + grid[x + 1][y - 1]

            # Check all combinations of words for both diagonals
            for word1 in words:
                if diag1 == word1:
                    for word2 in words:
                        if diag2 == word2:
                            # Found an X-MAS pattern
                            total_count += 1
                            # Debug: Uncomment the line below to see where matches are found
                            # print(f"Found X-MAS at center ({x}, {y}) with diagonals '{word1}' and '{word2}'")
    return total_count

def main():
    # Read the word search from input.txt
    with open('input.txt', 'r') as file:
        grid = [line.strip() for line in file if line.strip()]

    count = count_x_mas_occurrences(grid)
    print(f"The X-MAS appears {count} times in the word search.")

if __name__ == "__main__":
    main()
