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

fn main() {
    let lines = lines_from_file("input.txt");
    let mut adapters : Vec<_> = lines.iter().map(|l| l.parse::<i32>().unwrap()).collect();

    adapters.push(0);
    adapters.sort();
    adapters.push(adapters[adapters.len() - 1] + 3);

    let mut ones = 0;
    let mut threes = 0;

    for i in 1..adapters.len() {
        let diff = adapters[i] - adapters[i-1];
        if diff == 1 {
            ones += 1;
        } else if diff == 3 {
            threes += 1;
        }
    }

    let result = ones * threes;

    println!("Result: {}", result);
}
