#ifndef _DANKLE_H
#define _DANKLE_H

#define TERMINAL 0xFFFFFFF0
#define DEBUGGER 0xFFFFFFF2

#define FILESYSTEM 0xFFFFFF00
#define FS_MODE FILESYSTEM // byte
#define FS_TEXT FILESYSTEM + 1 // byte
#define FS_BUFF FILESYSTEM + 2 // short
#define FS_SIZE FILESYSTEM + 4 // int
#define FS_INDX FILESYSTEM + 8 // int

#define WRITE_CHAR_BUF(buf, c) *((char*)(buf)) = c
#define READ_INT_BUF(buf) *((int*)(buf))

#define WRITE_CHAR(c) WRITE_CHAR_BUF(TERMINAL, c)
#define BREAK() WRITE_CHAR_BUF(DEBUGGER, 1)

void println(const char* txt);
void printuln(const char* txt, char until);

#define readkey() *((char*)TERMINAL + 1)
char* readline(char* buf, int size);

char* itoa(int num, char* str, int base);

// Filesystem

/**
 * @brief Send command to filesystem driver
 * @param txt Command
 */
void fs_writetext(const char* txt);

/**
 * @brief Get the size of the loaded file
 * @return File size
 */
int fs_size();

#endif