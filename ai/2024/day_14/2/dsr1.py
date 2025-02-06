with open('input.txt') as f:
    lines = f.readlines()

robots = []
width = 101
height = 103

for line in lines:
    line = line.strip()
    p_part, v_part = line.split(' v=')
    # Parse position
    p_str = p_part[2:]  # Remove 'p='
    x_str, y_str = p_str.split(',')
    x = int(x_str)
    y = int(y_str)
    # Parse velocity
    vx, vy = map(int, v_part.split(','))
    robots.append((x, y, vx, vy))

min_area = float('inf')
best_t = 0

# Check a reasonable range of time steps
for t in range(0, 20000):
    x_coords = []
    y_coords = []
    for (x, y, vx, vy) in robots:
        x_t = (x + vx * t) % width
        y_t = (y + vy * t) % height
        x_coords.append(x_t)
        y_coords.append(y_t)
    
    # Calculate span for x
    sorted_x = sorted(x_coords)
    diffs_x = []
    if not sorted_x:
        span_x = 0
    else:
        for i in range(len(sorted_x)-1):
            diffs_x.append(sorted_x[i+1] - sorted_x[i])
        # Wrap-around difference
        diffs_x.append(sorted_x[0] + width - sorted_x[-1])
        max_gap_x = max(diffs_x)
        span_x = width - max_gap_x
    
    # Calculate span for y
    sorted_y = sorted(y_coords)
    diffs_y = []
    if not sorted_y:
        span_y = 0
    else:
        for i in range(len(sorted_y)-1):
            diffs_y.append(sorted_y[i+1] - sorted_y[i])
        # Wrap-around difference
        diffs_y.append(sorted_y[0] + height - sorted_y[-1])
        max_gap_y = max(diffs_y)
        span_y = height - max_gap_y
    
    area = span_x * span_y
    if area < min_area:
        min_area = area
        best_t = t

print(best_t)