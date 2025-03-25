#include <dankle.h>
#include <stdarg.h>

short add(short x, short y)
{
    return x + y;
}

short main()
{
    short (*func)(short, short) = add;
    short val = func(4, 7);

    char str[10];
    println(itoa(val, &str, 10));

    return 0;
}