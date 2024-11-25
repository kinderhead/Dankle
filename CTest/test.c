#include "lib.h"
//#include "printf.h"

void math(unsigned long x, unsigned long y)
{
    long ret = x / y;
    if (ret == 0x48D159E)
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
    char str[16];
    itoa(69, str, 10);
    println(str);
    //math(0x12345678, 4);
    return 0;
}
