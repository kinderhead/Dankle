#include <dankle.h>
#include <stdarg.h>

void test(short x, ...)
{
    va_list list;
    va_start(list, x);

    int y = va_arg(list, int);

    char str[10];
    println(itoa(y, &str, 10));
    y = va_arg(list, int);
    println(itoa(y, &str, 10));

    va_end(list);
}

short main()
{
    test(0, 4535, -34275);

    return 0;
}