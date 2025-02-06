from collections import defaultdict

def main():
    with open('input.txt') as f:
        grid = [list(line.strip()) for line in f]
        grid = [[int(c) for c in row] for row in grid]
    
    rows = len(grid)
    if rows == 0:
        print(0)
        return
    cols = len(grid[0])
    
    height_map = defaultdict(list)
    for i in range(rows):
        for j in range(cols):
            h = grid[i][j]
            height_map[h].append((i, j))
    
    dp = [[0] * cols for _ in range(rows)]
    
    for h in reversed(range(10)):
        for i, j in height_map.get(h, []):
            if h == 9:
                dp[i][j] = 1
            else:
                total = 0
                for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
                    x, y = i + dx, j + dy
                    if 0 <= x < rows and 0 <= y < cols:
                        if grid[x][y] == h + 1:
                            total += dp[x][y]
                dp[i][j] = total
    
    total_rating = 0
    for i in range(rows):
        for j in range(cols):
            if grid[i][j] == 0:
                total_rating += dp[i][j]
    
    print(total_rating)

if __name__ == "__main__":
    main()