use std::{
    collections::HashSet,
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

fn is_valid_passport(fields: &HashSet::<String>) -> bool {
    fields.len() == 8 || fields.len() == 7 && !fields.contains("cid")
}

fn main() {
    let mut result = 0;
    let lines = lines_from_file("input.txt");

    let mut set: HashSet::<String> = HashSet::new();

    for line in lines {
        if line.is_empty() {
            if is_valid_passport(&set) {
                result += 1;
            }
            set.clear();
        } else {
            let kvps = line.split(" ");
            for kvp_str in kvps {
                let mut kvp = kvp_str.split(":");
                let key = kvp.next().unwrap();
                set.insert(key.to_string());
            }
        }
    }
    if is_valid_passport(&set) {
        result += 1;
    }

    println!("Result: {}", result);
}
