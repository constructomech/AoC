#!/usr/bin/env python3
import re

def get_positions(points, t):
    """Return list of positions at time t given list of points.
       Each point is a tuple: (x, y, vx, vy)
    """
    return [(x + t * vx, y + t * vy) for (x, y, vx, vy) in points]

def bounding_box_area(positions):
    xs = [p[0] for p in positions]
    ys = [p[1] for p in positions]
    width = max(xs) - min(xs)
    height = max(ys) - min(ys)
    return width * height

def read_input(filename):
    points = []
    pattern = re.compile(r"p=(-?\d+),(-?\d+)\s+v=(-?\d+),(-?\d+)")
    with open(filename, "r") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            m = pattern.match(line)
            if m:
                x, y, vx, vy = map(int, m.groups())
                points.append((x, y, vx, vy))
    return points

def main():
    points = read_input("input.txt")
    
    # We assume that at t=0 the points are far apart, then they come together,
    # reach a minimum area, then spread apart again.
    # We step forward until the area starts increasing.
    t = 0
    best_t = 0
    best_area = float("inf")
    
    # Let’s use a simple simulation loop. You might need to adjust max_t if necessary.
    while True:
        pos = get_positions(points, t)
        area = bounding_box_area(pos)
        
        # If the area is the smallest we've seen, remember t.
        if area < best_area:
            best_area = area
            best_t = t
        else:
            # if the area has increased for this t, we assume we passed the minimum.
            # (One could add more checks to be sure.)
            # We break out of the loop.
            break
        
        t += 1

    print("Fewest seconds before the Easter egg appears:", best_t)

if __name__ == '__main__':
    main()
