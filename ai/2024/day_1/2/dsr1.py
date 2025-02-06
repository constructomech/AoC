from collections import Counter

def main():
    with open('input.txt') as f:
        left = []
        right = []
        for line in f:
            parts = line.strip().split()
            left.append(int(parts[0]))
            right.append(int(parts[1]))
    
    right_counts = Counter(right)
    total = sum(num * right_counts.get(num, 0) for num in left)
    print(total)

if __name__ == "__main__":
    main()