#ifndef _DANKLE_H
#define _DANKLE_H

#define TERMINAL 0xFFFFFFF0
#define DEBUGGER 0xFFFFFFF2
#define WRITE_CHAR(c) *((char*)TERMINAL) = c
#define BREAK() *((char*)DEBUGGER) = 1

void println(const char* txt);

#define readkey() *((char*)TERMINAL + 1)
char* readline(char* buf, int size);

char* itoa(int num, char* str, int base);

#endif