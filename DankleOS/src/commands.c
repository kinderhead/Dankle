#include "commands.h"

#define NUM_CMDS 3

typedef struct command_s
{
    void (*cb)(const char*);
    const char* name;
    const char* help;
} command_t;

extern char prompt[32];

static void about(const char* args)
{
    printf("______  ___   _   _  _   __ _      _____\n|  _  \\/ _ \\ | \\ | || | / /| |    |  ___|\n| | | / /_\\ \\|  \\| || |/ / | |    | |\n| | | |  _  || . ` ||    \\ | |    |  __|\n| |/ /| | | || |\\  || |\\  \\| |____| |___\n|___/ \\_| |_/\\_| \\_/\\_| \\_/\\_____/\\____/\n\n");
    printf("Dankle OS command line\n");
}

static void echo(const char* args)
{
    printf(args);
    WRITE_CHAR('\n');
}

static void set_prompt(const char* args)
{
    if (strlen(args) >= 32)
    {
        printf("Prompt too large\n");
    }
    else
    {
        strcpy(prompt, args);
        //WRITE_CHAR('\n');
    }
}

static void help(const char* args)
{
    
}

command_t cmds[NUM_CMDS] = {
    { about, "about", "Display the about message." },
    { echo, "echo", "Print arguments to the console.\nUSAGE: echo <message>" },
    { set_prompt, "prompt", "Set the prompt text.\nUSAGE: set_prompt <prompt>" }
};

void run_cmd(const char* cmd, const char* args)
{
    for (int i = 0; i < NUM_CMDS; i++)
    {
        if (streq(cmd, cmds[i].name))
        {
            cmds[i].cb(args);
            return;
        }
    }

    printf("Invalid command \"%s\"\n", cmd);
}
