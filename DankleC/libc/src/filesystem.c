#include <dankle.h>

void fs_writetext(const char* txt, bool finish)
{
    while (*txt != 0)
    {
        WRITE_CHAR_BUF(FS_TEXT, *txt++);
    }

    if (finish) WRITE_CHAR_BUF(FS_TEXT, 0);
}

bool fs_open(const char* txt, int mode)
{
    fs_setmode(mode);
    fs_writetext(txt, true);

    if (fs_err() != 0) return false;
    return true;
}

bool fs_mkdir(const char* path)
{
    fs_setmode(FS_MODE_MKDIR);
    fs_writetext(path, true);
    
    if (fs_err() != 0) return false;
    return true;
}

int fs_read(char* data, int size)
{
    short b;
    int index = 0;

    while ((b = READ_SHORT_BUF(FS_BUFF)) != -1 && index < size)
    {
        data[index++] = (char) b;
    }

    return index;
}

void fs_write(char* data, int size, bool close)
{
    for (int i = 0; i < size; i++)
    {
        WRITE_SHORT_BUF(FS_BUFF, data[i]);
    }

    if (close) WRITE_SHORT_BUF(FS_BUFF, -1);
}

int fs_size()
{
    return READ_INT_BUF(FS_SIZE);
}

int fs_err()
{
    return READ_CHAR_BUF(FS_ERRC);
}

int fs_checkerr()
{
    // int err = fs_err();
    // switch (err)
    // {
    // case FS_ERR_NONE:
    //     return 0;

    // case FS_ERR_NOTFOUND:
        
    //     break;

    // default:
    //     break;
    // }

    return 1;
}

void fs_setmode(int mode)
{
    WRITE_CHAR_BUF(FS_MODE, mode);
}
