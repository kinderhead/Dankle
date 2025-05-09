#ifndef _DANKLE_H
#define _DANKLE_H

#include <stdbool.h>

#define TERMINAL 0xFFFFFFF0
#define DEBUGGER 0xFFFFFFF6

#define FILESYSTEM 0xFFFFFF00
#define FS_MODE FILESYSTEM // byte
#define FS_TEXT FILESYSTEM + 1 // byte
#define FS_BUFF FILESYSTEM + 2 // short
#define FS_SIZE FILESYSTEM + 4 // int
#define FS_INDX FILESYSTEM + 8 // int
#define FS_ERRC FILESYSTEM + 12 // byte

#define FS_MODE_READ 0
#define FS_MODE_WRITE 1
#define FS_MODE_MKDIR 2
#define FS_MODE_PATH 3

#define FS_ERR_NONE 0
#define FS_ERR_NOTFOUND 1
#define FS_ERR_NOTWRITING 2

#define WRITE_CHAR_BUF(buf, c) *((char*)(buf)) = c
#define WRITE_SHORT_BUF(buf, c) *((short*)(buf)) = c
#define READ_INT_BUF(buf) *((int*)(buf))
#define READ_SHORT_BUF(buf) *((short*)(buf))
#define READ_CHAR_BUF(buf) *((char*)(buf))

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
 * @param finish Send null terminator to driver
 */
void fs_writetext(const char* txt, bool finish);

/**
 * @brief Open file
 * @param txt Filepath
 * @param mode Mode
 * @return Operation succeeded
 */
bool fs_open(const char* txt, int mode);

/**
 * @brief Make directory or do nothing if it exists
 * @return Operation succeeded
 */
bool fs_mkdir(const char* path);

/**
 * @brief Resolve path
 * @param path Path to resolve
 * @param dest Destination buffer
 * @param maxlen Maximum length to read
 * @return Resolved path
 */
char* fs_compress_path(const char* path, char* dest, int maxlen);

/**
 * @brief Read next n bytes from loaded file
 * @param data Pointer to destination
 * @param size Size to read
 * @return Bytes read
 */
int fs_read(char* data, int size);

/**
 * @brief Write bytes to loaded file
 * @param data Pointer to source
 * @param size Number of bytes to write
 * @param close Close file after writing
 */
void fs_write(char* data, int size, bool close);

/**
 * @brief Get the size of the loaded file
 * @return File size
 */
int fs_size();

/**
 * @brief Get error code for last command
 * @return Error code
 */
int fs_err();

/**
 * @brief Print appropriate error messages or do nothing (WIP)
 * @return Whether or not the previous command succeeded
 */
int fs_checkerr();

/**
 * @brief Set filesystem driver mode
 * @param mode Mode
 */
void fs_setmode(int mode);

#endif