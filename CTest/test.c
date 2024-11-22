#include "lib.h"
#include "printf.h"

void math(long x, long y)
{
    long ret = x - y;
    if (ret == 0xFFFF)
    {
        println("Yay");
    }
    else if (ret == 0xFFF60005)
    {
        println("uh oh");
    }
}

int main()
{
    printf("Hello printf!\n");
    //math(0x10000, 1);
    return 0;
}
