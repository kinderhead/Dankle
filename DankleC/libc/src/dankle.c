#include <dankle.h>
#include <stdbool.h>

void println(const char* txt)
{
    while (*txt != 0)
    {
        WRITE_CHAR(*txt++);
    }
    WRITE_CHAR('\n');
}
