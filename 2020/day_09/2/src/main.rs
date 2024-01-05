use std::{
    fs::File,
    io::BufRead,
    io::BufReader,
    path::Path,
};

fn lines_from_file(filename: impl AsRef<Path>) -> Vec<String> {
    let file = File::open(filename).expect("no such file");
    let buf = BufReader::new(file);
    buf.lines()
       .map(|l| l.expect("Could not parse line"))
       .collect()
}

fn exists_sum(numbers: &[i64], target: i64) -> bool {
    for i in 0..numbers.len() {
        for j in 0..numbers.len() {
            if i == j {
                continue;
            }

            if numbers[i] + numbers[j] == target {
                return true;
            }
        }
    }

    false
}

fn main() {
    const PREAMBLE: usize = 25;
    let mut result : i64 = 0;
    let mut target_number : i64 = 0;
    let mut target_index : usize = 0;
    let lines = lines_from_file("input.txt");

    let numbers: Vec<i64> = lines.iter().map(|l| l.parse::<i64>().unwrap()).collect();

    for (i, _) in lines.iter().enumerate() {
        if i < PREAMBLE {
            continue;
        }

        if !exists_sum(&numbers[i-PREAMBLE..i], numbers[i]) {
            target_number = numbers[i];
            target_index = i;
            break;
        }
    }

    while target_index > 0 {
        let mut sum = 0;
        let mut j = target_index;

        while sum < target_number {
            j -= 1;
            sum += numbers[j];
        }

        if sum == target_number {
            let mut min = numbers[j];
            let mut max = numbers[j];

            for k in j..target_index {
                if numbers[k] < min {
                    min = numbers[k];
                }

                if numbers[k] > max {
                    max = numbers[k];
                }
            }

            result = min + max;
            break;
        }

        target_index -= 1;
    }

    println!("Result: {}", result);
}
