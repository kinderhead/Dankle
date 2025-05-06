#include <dankle.h>
#include <stdbool.h>
#include <stdio.h>

void println(const char* txt)
{
    while (*txt != 0)
    {
        WRITE_CHAR(*txt++);
    }
    WRITE_CHAR('\n');
}

void printuln(const char* txt, char until)
{
    while (*txt != 0 && *txt != until)
    {
        WRITE_CHAR(*txt++);
    }
    WRITE_CHAR('\n');
}

static void reverse(char* str, int length)
{
    int start = 0;
    int end = length - 1;
    while (start < end)
    {
        char temp = str[start];
        str[start] = str[end];
        str[end] = temp;
        end--;
        start++;
    }
}

// https://www.geeksforgeeks.org/implement-itoa/
char* itoa(int num, char* str, int base)
{
    int i = 0;
    bool isNegative = false;

    if (num == 0)
    {
        str[i++] = '0';
        str[i] = '\0';
        return str;
    }

    if (num < 0 && base == 10)
    {
        isNegative = true;
        num = -num;
    }

    while (num != 0)
    {
        int rem = num % base;
        str[i++] = (rem > 9) ? (rem - 10) + 'a' : rem + '0';
        num = num / base;
    }

    if (isNegative)
    {
        str[i++] = '-';
    }

    str[i] = '\0';

    reverse(str, i);

    return str;
}

void _putchar(char character)
{
    WRITE_CHAR(character);
}

char* readline(char* buf, int size)
{
    *((char*) TERMINAL + 3) = 0;

    int i;
    for (i = 0; i < size - 1; i++)
    {
        char c = *((char*) TERMINAL + 2);
        if (c == 0) break;
        buf[i] = c;
    }

    buf[i] = 0;

    return buf;
}
