#include <dankle.h>
#include <stdbool.h>

static void reverse(char* str, short length)
{
    short start = 0;
    short end = length - 1;
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
char* itoa(int num, char* str, short base)
{
    short i = 0;
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


short main()
{
    char str[10];
    println(itoa(69, &str, 10));

    return 0;
}