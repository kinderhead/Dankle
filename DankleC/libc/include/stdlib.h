#ifndef _STDLIB_H
#define _STDLIB_H

#include <stddef.h>

int atoi(char* s);

void* malloc(size_t size);
void free(void* ptr);

#endif