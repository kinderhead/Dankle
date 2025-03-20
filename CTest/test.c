#include <dankle.h>

static short test()
{
    static short x;
    return x++;
}

short main()
{
    if (test() == 0) println("Yay");
    if (test() == 1) println("Yay");
    if (test() == 2) println("Yay");
    return 0;
}