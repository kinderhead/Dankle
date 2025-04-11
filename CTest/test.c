#include <string.h>
#include <stdio.h>
#include <dankle.h>
#include <stdbool.h>

void main()
{
    while (true)
    {
        char cmdbuf[128];
        printf("> ");
        readline(cmdbuf, 128);

        char* cmd = strtok(cmdbuf, " ");
        printf(cmd);
    }
}
