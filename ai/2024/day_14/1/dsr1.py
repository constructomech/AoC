with open('input.txt') as f:
    lines = f.readlines()

tl = 0  # Top-Left quadrant count
tr = 0  # Top-Right quadrant count
bl = 0  # Bottom-Left quadrant count
br = 0  # Bottom-Right quadrant count

width = 101
height = 103

mid_x = (width - 1) // 2  # 50 for width=101
mid_y = (height - 1) // 2  # 51 for height=103

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
    # Calculate new position after 100 seconds with modulo
    x_new = (x + vx * 100) % width
    y_new = (y + vy * 100) % height
    
    # Determine quadrant
    if x_new < mid_x:
        if y_new < mid_y:
            tl += 1
        elif y_new > mid_y:
            bl += 1
    elif x_new > mid_x:
        if y_new < mid_y:
            tr += 1
        elif y_new > mid_y:
            br += 1

safety_factor = tl * tr * bl * br
print(safety_factor)