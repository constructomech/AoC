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
        for (j, y) in input.iter().enumerate().skip(i) {
            for z in input.iter().skip(j) {
                let x: i32 = x.parse().unwrap();
                let y: i32 = y.parse().unwrap();
                let z: i32 = z.parse().unwrap();

                // Check if the sum is 2020
                if x + y + z == 2020 {
                    result = x * y * z;
                    break 'outer;
                }
            }
        }
    }

    println!("Result: {}", result);
}
