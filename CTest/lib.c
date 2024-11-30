#include "lib.h"

inline void _putchar(char c)
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

void itoa(int num, char* str, int base)
{
    //BREAK();
    int i = 0;
    int isNegative = 0;

    if (num == 0) {
        str[i++] = '0';
        str[i] = '\0';
        return;
    }
 
    if (num < 0 && base == 10) {
        isNegative = 1;
        num = -num;
    }
 
    while (num != 0) {
        int rem = num % base;
        str[i++] = (rem > 9) ? (rem - 10) + 'a' : rem + '0';
        num = num / base;
    }

    if (isNegative)
        str[i++] = '-';
 
    str[i] = '\0';
 
    int start = 0;
    int end = i - 1;
    while (start < end) {
        char temp = str[start];
        str[start] = str[end];
        str[end] = temp;
        end--;
        start++;
    }
}
