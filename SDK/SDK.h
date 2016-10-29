#pragma once

void _start();
void main();

void _start() {
	main();
}

// Define intrinsics
#define __trap(n) __asm__("trap #" #n ";")

// Custom intrinsics
#define __bullshit() __asm__(".byte 0b11111111, 0b11111111;")

typedef unsigned char byte;
