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

    for line in lines {
        let (row, col) = decode_seat(&line);
        let seat_id = row * 8 + col;
        //println!("{}: row {}, col {}, seat ID {}", line, row, col, seat_id);

        result = result.max(seat_id);
    }

    println!("Result: {}", result);
}
