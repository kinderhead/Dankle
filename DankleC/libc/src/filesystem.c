#include <dankle.h>

void fs_writetext(const char* txt)
{
    do
    {
        WRITE_CHAR_BUF(FS_TEXT, *txt++);
    }
    while (*txt != 0);
}
