#include <dankle.h>

short main()
{
    int x = -4;
    int y = 1;

    x >>= y;

    char str[10];
    println(itoa(x, &str, 10));

    return 0;
}