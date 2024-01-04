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

#[derive(Clone)]
struct Instruction {
    name: String,
    value: i32,
}

fn run_program(instructions: &Vec<Instruction>) -> Result<i32, bool> {
    let mut instruction_pointer : i32 = 0;
    let mut accumulator = 0;
    let mut visited_instructions : HashSet::<i32> = HashSet::new();

    while !visited_instructions.contains(&instruction_pointer) {
        visited_instructions.insert(instruction_pointer);

        if instruction_pointer < 0 || instruction_pointer as usize > instructions.len() {
            return Err(false);
        } else if instruction_pointer as usize == instructions.len() {
            return Ok(accumulator);
        } 

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

    return Err(false);
}

fn generate_variations(instructions: &Vec<Instruction>) -> impl Iterator<Item = Vec<Instruction>> + '_ {
    (0..instructions.len()).flat_map(move |i| {
        let mut variation = instructions.clone();
        let instruction = &variation[i];

        match instruction.name.as_str() {
            "nop" => variation[i].name = "jmp".to_string(),
            "jmp" => variation[i].name = "nop".to_string(),
            _ => return None, // Skip non-"nop" and non-"jmp" instructions
        }

        Some(variation)
    })
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

    for variation in generate_variations(&instructions) {
        if let Ok(result) = run_program(&variation) {
            println!("Result: {}", result);
            break;
        }
    }
}
