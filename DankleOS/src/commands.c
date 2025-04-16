#include "commands.h"

#define DEFINE_CMD(name, help) static const char* name##_help = help;
#define RUN_CMD(name) else if (streq(cmd, #name)) name(args)

DEFINE_CMD(about, "Display the about message.")
DEFINE_CMD(echo, "Print arguments to the console.\nUSAGE: echo <message>")
DEFINE_CMD(set_prompt, "Set the prompt text.\nUSAGE: set_prompt <prompt>")

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

void run_cmd(const char* cmd, const char* args)
{
    if (cmd[0] == 0) printf("Empty command\n");
    RUN_CMD(about);
    RUN_CMD(echo);
    RUN_CMD(set_prompt);
    else printf("Invalid command \"%s\"\n", cmd);
}
