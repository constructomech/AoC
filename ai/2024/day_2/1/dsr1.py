def count_safe_reports(reports):
    count = 0
    for line in reports:
        nums = list(map(int, line.strip().split()))
        if len(nums) < 2:
            count += 1
            continue
        
        # Check if strictly increasing
        is_increasing = True
        for i in range(len(nums)-1):
            if nums[i+1] <= nums[i]:
                is_increasing = False
                break
        
        # Check if strictly decreasing
        is_decreasing = True
        for i in range(len(nums)-1):
            if nums[i+1] >= nums[i]:
                is_decreasing = False
                break
        
        # Skip if neither strictly increasing nor decreasing
        if not (is_increasing or is_decreasing):
            continue
        
        # Validate differences between adjacent elements
        valid = True
        if is_increasing:
            for i in range(len(nums)-1):
                diff = nums[i+1] - nums[i]
                if not (1 <= diff <= 3):
                    valid = False
                    break
        else:
            for i in range(len(nums)-1):
                diff = nums[i] - nums[i+1]
                if not (1 <= diff <= 3):
                    valid = False
                    break
        
        if valid:
            count += 1
    
    return count

# Read the input data from input.txt
with open('input.txt', 'r') as file:
    input_data = file.read().splitlines()

# Calculate and print the result
print(count_safe_reports(input_data))