cmake_minimum_required(VERSION 3.23)
project(cpp2_sandbox)

include(FetchContent)

add_subdirectory(src_deps/cppfront_cmake)

# cppfront flags
# -p - "Pure" cpp2, disable cpp1 mixing
# -format-colon-errors - Emit ':line:col:' format for messages 
set(CPPFRONT_FLAGS "-p" "-format-colon-errors")

add_executable(main src/main.cpp2)