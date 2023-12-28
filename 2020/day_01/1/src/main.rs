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
    let mut result = 0;

    // Read in the file input.txt
    let input = lines_from_file("input.txt");

    // Iterate over all unique pairs of numbers
    'outer: for (i, x) in input.iter().enumerate() {
        for y in input.iter().skip(i) {
            // Convert the strings to numbers
            let x: i32 = x.parse().unwrap();
            let y: i32 = y.parse().unwrap();

            // Check if the sum is 2020
            if x + y == 2020 {
                result = x * y;
                break 'outer;
            }
        }
    }

    println!("Rewsult: {}", result);
}
