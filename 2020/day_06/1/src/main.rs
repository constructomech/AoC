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

fn count_bits(num: u32) -> u32 {
    let mut result = 0;
    let mut n = num;

    while n > 0 {
        result += n & 1;
        n >>= 1;
    }

    result
}

fn main() {
    let mut result = 0;
    let mut lines = lines_from_file("input.txt");
    lines.push(String::new()); // To make the loop iteration simpler

    let mut questions: u32 = 0;

    for line in lines {
        if line == "" {
            result += count_bits(questions);
            questions = 0;
            continue;
        }
        for c in line.chars() {
            let bit_num : u32 = c as u32 - 'a' as u32;
            questions |= 1 << bit_num;
        }
    }

    println!("Result: {}", result);
}
