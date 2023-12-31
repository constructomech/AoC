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

fn decode_seat(line: &str) -> (i32, i32) {
    let mut row = 0;
    let mut col = 0;

    for (i, c) in line.chars().enumerate() {
        match c {
            'F' => row <<= 1,
            'B' => row = (row | 1) << 1,
            'L' => col <<= 1,
            'R' => col = (col | 1) << 1,
            _ => panic!("Invalid character: {}", c),
        }
        if i == line.len() - 1 {
            row >>= 1;
            col >>= 1;
        }
    }

    (row, col)
}

fn main() {
    let mut result = 0;
    let lines = lines_from_file("input.txt");

    let seats = lines.iter().map(|line| decode_seat(&line)).collect::<Vec<_>>();
    let min_row = seats.iter().map(|&(row, _)| row).min().unwrap_or_default();
    let max_row = seats.iter().map(|&(row, _)| row).max().unwrap_or_default();
    let max_col = seats.iter().map(|&(_, col)| col).max().unwrap_or_default();

    'out: for row in min_row + 1..max_row - 1 {
        for col in 0..=max_col {
            if !seats.contains(&(row, col)) {
                result = row * 8 + col;
                break 'out;
            }
        }
    }

    println!("Result: {}", result);
}
