#ifndef _STRING_H
#define _STRING_H

#include <stddef.h>

int strcmp(const char* a, const char* b);

char* strtok(char* s, const char* delim);
char* strtok_r(char* s, const char* delim, char** save_ptr);

#endif
