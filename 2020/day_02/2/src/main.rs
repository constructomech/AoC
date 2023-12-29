use std::{
    fs::File,
    io::BufRead,
    io::BufReader,
    path::Path,
};

use regex::Regex;

fn lines_from_file(filename: impl AsRef<Path>) -> Vec<String> {
    let file = File::open(filename).expect("no such file");
    let buf = BufReader::new(file);
    buf.lines()
       .map(|l| l.expect("Could not parse line"))
       .collect()
}

fn main() {
    let mut result = 0;
    let lines = lines_from_file("input.txt");

    let re = Regex::new(r"(\d+)-(\d+) ([a-z]): (.+)").unwrap();

    for line in lines {
        if let Some(captures) = re.captures(line.as_str()) {
            let first_pos = captures[1].parse::<usize>().unwrap() - 1;
            let second_pos = captures[2].parse::<usize>().unwrap() - 1;
            let character = captures[3].chars().next().unwrap();
            let password = captures[4].to_string();

            //println!("{}-{} {}: {}", start, end, character, password);
            let a = password.chars().nth(first_pos).unwrap();
            let b = password.chars().nth(second_pos).unwrap();

            if a != b && (a == character || b == character) {
                result += 1;
            }
        }
    }

    println!("Result: {}", result);
}
