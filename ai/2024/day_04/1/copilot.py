def count_xmas_occurrences(grid):
    rows = len(grid)
    cols = len(grid[0])
    word = "XMAS"
    
    # Directions: N, NE, E, SE, S, SW, W, NW
    directions = [
        (-1, 0),   # N
        (-1, 1),   # NE
        (0, 1),    # E
        (1, 1),    # SE
        (1, 0),    # S
        (1, -1),   # SW
        (0, -1),   # W
        (-1, -1),  # NW
    ]
    
    total_count = 0
    
    for row in range(rows):
        for col in range(cols):
            for dir_x, dir_y in directions:
                match = True
                for k in range(len(word)):
                    new_row = row + dir_x * k
                    new_col = col + dir_y * k
                    if 0 <= new_row < rows and 0 <= new_col < cols:
                        if grid[new_row][new_col] != word[k]:
                            match = False
                            break
                    else:
                        match = False
                        break
                if match:
                    total_count += 1
    return total_count

def main():
    # Read the word search from input.txt
    with open('input.txt', 'r') as file:
        grid = [line.strip() for line in file if line.strip()]
    
    count = count_xmas_occurrences(grid)
    print(f"The word 'XMAS' appears {count} times in the word search.")

if __name__ == "__main__":
    main()
