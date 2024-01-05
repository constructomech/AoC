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

fn count_paths(adapters: &Vec<i32>, index: usize, cache: &mut Vec<i64>) -> i64 {
    if index == adapters.len() - 1 {
        return 1;
    }

    if cache[index] != -1 {
        return cache[index];
    }

    let mut result = 0;

    for i in index+1..adapters.len() {
        if adapters[i] - adapters[index] <= 3 {
            result += count_paths(adapters, i, cache);
        } else {
            break;
        }
    }

    cache[index] = result;

    return result;
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

    let result = count_paths(&adapters, 0, &mut vec![-1; adapters.len()]);

    println!("Result: {}", result);
}
