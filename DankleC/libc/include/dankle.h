#ifndef _DANKLE_H
#define _DANKLE_H

#define TERMINAL 0xFFFFFFF0
#define DEBUGGER 0xFFFFFFF2

#define FILESYSTEM 0xFFFFFF00
#define FS_MODE FILESYSTEM // byte
#define FS_TEXT FILESYSTEM + 1 // byte
#define FS_BUFF FILESYSTEM + 2 // short
#define FS_INDX FILESYSTEM + 4 // int

#define WRITE_CHAR_BUF(buf, c) *((char*)buf) = c
#define WRITE_CHAR(c) WRITE_CHAR_BUF(TERMINAL, c)
#define BREAK() WRITE_CHAR_BUF(TERMINAL, 1)

void println(const char* txt);
void printuln(const char* txt, char until);

#define readkey() *((char*)TERMINAL + 1)
char* readline(char* buf, int size);

char* itoa(int num, char* str, int base);

void fs_writetext(const char* txt);

#endif