#include <dankle.h>

short main()
{
    int x = 2147483647;
    int y = 3;

    x %= y;

    char str[10];
    println(itoa(x, &str, 10));

    return 0;
}