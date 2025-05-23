#ifndef _STRING_H
#define _STRING_H

#include <stddef.h>

int strcmp(const char* a, const char* b);
#define streq(a, b) strcmp(a, b) == 0 // Custom

size_t strlen(const char* str);

char* strtok(char* s, const char* delim);
char* strtok_r(char* s, const char* delim, char** save_ptr);
char* strcpy(char* strDest, const char* strSrc);
char* strcat(char* dest, const char* src);

#endif
