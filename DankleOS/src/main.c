#include "commands.h"
#include <stdlib.h>

char prompt[32];

void main()
{
    printf("Dankle OS\n\n");

    while (true)
    {
        char cmdbuf[512];
        printf("%s> ", prompt);

        readline(cmdbuf, 128);

        if (cmdbuf[0] == 0) continue;

        const char* args;
        char* cmd = strtok_r(cmdbuf, " ", &args);

        run_cmd(cmd, args);
    }
}
