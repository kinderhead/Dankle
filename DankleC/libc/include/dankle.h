#ifndef _DANKLE_H
#define _DANKLE_H

#define TERMINAL 0xFFFFFFF0
#define WRITE_CHAR(c) *((char*)0xFFFFFFF0) = c

void println(const char* txt);
char* itoa(int num, char* str, short base);

#endif