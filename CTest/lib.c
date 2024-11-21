#include "lib.h"

void _putchar(char c)
{
    *((unsigned char*)(0xFFFFFFF0u)) = c;
}

void println(const char* str)
{
    while (*str != 0)
    {
        _putchar(*str);
        str++;
    }

    _putchar('\n');
}
