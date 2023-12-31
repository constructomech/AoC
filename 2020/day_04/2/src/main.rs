use std::{
    collections::HashMap,
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

fn is_numeric_field_valid(fields: &HashMap::<String, String>, name: &str, min: i32, max: i32) -> bool {
    if let Some(value) = fields.get(name) {
        if let Ok(num) = value.parse::<i32>() {
            return num >= min && num <= max;
        }
    }
    false
}

fn is_height_field_valid(fields: &HashMap::<String, String>, name: &str) -> bool {
    if let Some(value) = fields.get(name) {
        if let Ok(num) = value[..value.len()-2].parse::<i32>() {
            if value.ends_with("cm") {
                return num >= 150 && num <= 193;
            } else if value.ends_with("in") {
                return num >= 59 && num <= 76;
            }
        }
    }
    false
}

fn is_hair_color_field_valid(fields: &HashMap::<String, String>, name: &str) -> bool {
    if let Some(value) = fields.get(name) {
        if value.starts_with("#") && value.len() == 7 {
            for c in value[1..].chars() {
                if !c.is_digit(16) {
                    return false;
                }
            }
            return true;
        }
    }
    false
}

fn is_eye_color_field_valid(fields: &HashMap::<String, String>, name: &str) -> bool {
    const VALID_EYE_COLORS: [&str; 7] = ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"];

    if let Some(value) = fields.get(name) {
        return VALID_EYE_COLORS.contains(&value[..].as_ref());
    }
    false
}

fn is_passport_id_field_valid(fields: &HashMap::<String, String>, name: &str) -> bool {
    if let Some(value) = fields.get(name) {
        if value.len() == 9 {
            for c in value.chars() {
                if !c.is_digit(10) {
                    return false;
                }
            }
            return true;
        }
    }
    false
}

// byr (Birth Year) - four digits; at least 1920 and at most 2002.
// iyr (Issue Year) - four digits; at least 2010 and at most 2020.
// eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
// hgt (Height) - a number followed by either cm or in:
// If cm, the number must be at least 150 and at most 193.
// If in, the number must be at least 59 and at most 76.
// hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
// ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
// pid (Passport ID) - a nine-digit number, including leading zeroes.
// cid (Country ID) - ignored, missing or not.
fn is_valid_passport(fields: &HashMap::<String, String>) -> bool {
    if !(fields.len() == 8 || fields.len() == 7 && !fields.contains_key("cid")) {
        return false;
    }

    if !is_numeric_field_valid(fields, "byr", 1920, 2002) {
        return false;
    }

    if !is_numeric_field_valid(fields, "iyr", 2010, 2020) {
        return false;
    }

    if !is_numeric_field_valid(fields, "eyr", 2020, 2030) {
        return false;
    }

    if !is_height_field_valid(fields, "hgt") {
        return false;
    }

    if !is_hair_color_field_valid(fields, "hcl") {
        return false;
    }

    if !is_eye_color_field_valid(fields, "ecl") {
        return false;
    }

    if !is_passport_id_field_valid(fields, "pid") {
        return false;
    }

    true
}

fn main() {
    let mut result = 0;
    let lines = lines_from_file("input.txt");

    let mut set: HashMap::<String, String> = HashMap::new();

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
                let value = kvp.next().unwrap();
                set.insert(key.to_string(), value.to_string());
            }
        }
    }
    if is_valid_passport(&set) {
        result += 1;
    }

    println!("Result: {}", result);
}
