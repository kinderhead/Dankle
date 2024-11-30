#ifndef _LIB_H
#define _LIB_H

#define BREAK() *((unsigned char*)(0xFFFFFFF2u)) = 0;

//void _putchar(char c);
void println(const char* str);
void itoa(int num, char* str, int base);

#endif
