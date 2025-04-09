#include <string.h>
#include <dankle.h>

// https://stackoverflow.com/questions/34873209/implementation-of-strcmp
int strcmp(const char* a, const char* b)
{
    while (*a && (*a == *b))
    {
        a++;
        b++;
    }

    return *(const unsigned char*) a - *(const unsigned char*) b;
}
