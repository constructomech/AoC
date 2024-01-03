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

struct Instruction {
    name: String,
    value: i32,
}

fn run_program(instructions: &Vec<Instruction>) -> i32 {
    let mut accumulator = 0;
    let mut instruction_pointer = 0;
    let mut visited_instructions : HashSet::<i32> = HashSet::new();

    while !visited_instructions.contains(&instruction_pointer) {
        visited_instructions.insert(instruction_pointer);

        let instruction = &instructions[instruction_pointer as usize];
        match instruction.name.as_str() {
            "nop" => instruction_pointer += 1,
            "acc" => {
                accumulator += instruction.value;
                instruction_pointer += 1;
            },
            "jmp" => instruction_pointer += instruction.value,
            _ => panic!("Unknown instruction {}", instruction.name),
        }
    }

    return accumulator;
}

fn main() {
    let lines = lines_from_file("input.txt");

    let mut instructions = Vec::new();

    for line in lines {
        let mut parts = line.split_whitespace();
        let instruction = parts.next().unwrap();
        let value = parts.next().unwrap().parse::<i32>().unwrap();
        instructions.push(Instruction{name: instruction.to_string(), value: value});
    }

    let result = run_program(&instructions);
    println!("Result: {}", result);
}
