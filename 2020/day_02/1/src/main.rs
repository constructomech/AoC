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
            let start = captures[1].parse::<i32>().unwrap();
            let end = captures[2].parse::<i32>().unwrap();
            let character = captures[3].chars().next().unwrap();
            let password = captures[4].to_string();

            //println!("{}-{} {}: {}", start, end, character, password);

            let count = password.matches(character).count() as i32;

            if count >= start && count <= end {
                result += 1;
            }
        }
    }

    println!("Result: {}", result);
}
