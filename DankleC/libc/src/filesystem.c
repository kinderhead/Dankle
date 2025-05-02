#include <dankle.h>

void fs_writetext(const char* txt)
{
    while (*txt != 0)
    {
        WRITE_CHAR_BUF(FS_TEXT, *txt++);
    }
    WRITE_CHAR_BUF(FS_TEXT, 0);
}

int fs_size()
{
    return READ_INT_BUF(FS_SIZE);
}
