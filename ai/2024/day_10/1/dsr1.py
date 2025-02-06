from collections import defaultdict

def main():
    with open('input.txt') as f:
        grid = [list(map(int, line.strip())) for line in f]
    
    rows = len(grid)
    if rows == 0:
        print(0)
        return
    cols = len(grid[0])
    
    trailheads = []
    nines = []
    for i in range(rows):
        for j in range(cols):
            if grid[i][j] == 0:
                trailheads.append((i, j))
            elif grid[i][j] == 9:
                nines.append((i, j))
    
    scores = defaultdict(int)
    
    for (x9, y9) in nines:
        visited = set()
        current = set()
        current.add((x9, y9))
        visited.add((x9, y9))
        
        for h in reversed(range(9)):  # h from 8 down to 0
            next_current = set()
            for (x, y) in current:
                for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
                    nx = x + dx
                    ny = y + dy
                    if 0 <= nx < rows and 0 <= ny < cols:
                        if grid[nx][ny] == h and (nx, ny) not in visited:
                            visited.add((nx, ny))
                            next_current.add((nx, ny))
            current = next_current
        
        for (x, y) in current:
            if grid[x][y] == 0:
                scores[(x, y)] += 1
    
    total = sum(scores.values())
    print(total)

if __name__ == "__main__":
    main()