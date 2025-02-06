#!/usr/bin/env python3
import re

def main():
    # Grid dimensions
    WIDTH = 101
    HEIGHT = 103

    # Middle coordinates (robots on these lines/columns are excluded)
    middle_x = WIDTH // 2  # 101//2 == 50
    middle_y = HEIGHT // 2  # 103//2 == 51

    # These will store the count of robots in each quadrant
    count_top_left = 0
    count_top_right = 0
    count_bottom_left = 0
    count_bottom_right = 0

    # Regular expression to parse lines of the form:
    # p=0,4 v=3,-3
    line_re = re.compile(r"p=(-?\d+),(-?\d+)\s+v=(-?\d+),(-?\d+)")

    with open("input.txt", "r") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            match = line_re.match(line)
            if not match:
                continue
            x, y, vx, vy = map(int, match.groups())

            # Compute position after 100 seconds using modular arithmetic
            new_x = (x + 100 * vx) % WIDTH
            new_y = (y + 100 * vy) % HEIGHT

            # If the robot lands exactly on the vertical or horizontal middle,
            # then skip it.
            if new_x == middle_x or new_y == middle_y:
                continue

            # Determine which quadrant
            if new_x < middle_x and new_y < middle_y:
                count_top_left += 1
            elif new_x > middle_x and new_y < middle_y:
                count_top_right += 1
            elif new_x < middle_x and new_y > middle_y:
                count_bottom_left += 1
            elif new_x > middle_x and new_y > middle_y:
                count_bottom_right += 1

    # Compute the safety factor as the product of counts in the four quadrants.
    safety_factor = count_top_left * count_top_right * count_bottom_left * count_bottom_right

    print("Safety factor:", safety_factor)

if __name__ == '__main__':
    main()
