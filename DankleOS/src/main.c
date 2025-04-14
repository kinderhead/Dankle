#include "commands.h"

void main()
{
    printf("Dankle OS\n\n");

    while (true)
    {
        char cmdbuf[512];
        printf("> ");

        readline(cmdbuf, 128);

        if (cmdbuf[0] == 0) continue;

        const char* args;
        char* cmd = strtok_r(cmdbuf, " ", &args);

        run_cmd(cmd, args);
    }
}
