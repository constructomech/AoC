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

fn traverse(lines: &Vec<String>, right: usize, down: usize) -> usize {
    let mut result = 0;
    let mut x = 0;
    let mut y = 0;
    let width = lines[0].len();
    let height = lines.len();

    while y < height {
        if lines[y].chars().nth(x).unwrap() == '#' {
            result += 1;
        }
        x = (x + right) % width;
        y += down;
    }

    result
}

fn main() {
    let lines = lines_from_file("input.txt");

    let result = traverse(&lines, 3, 1);

    println!("Result: {}", result);
}
