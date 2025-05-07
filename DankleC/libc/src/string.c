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

// https://stackoverflow.com/questions/14476627/strcpy-implementation-in-c
char* strcpy(char* strDest, const char* strSrc)
{
    char* temp = strDest;
    while (*strDest++ = *strSrc++) { }
    return temp;
}

char* strcat(char* dest, const char* src)
{
    strcpy(dest + strlen(dest), src);
    return dest;
}

size_t strlen(const char* str)
{
    const char* p;

    if (str == NULL)
        return 0;

    p = str;
    while (*p != '\0')
        p++;

    return p - str;
}

// The following is yoinked from OpenBSD's libc

char* strtok(char* s, const char* delim)
{
    static char* last;

    return strtok_r(s, delim, &last);
}

char* strtok_r(char* s, const char* delim, char** last)
{
    const char* spanp;
    int c, sc;
    char* tok;

    if (s == NULL && (s = *last) == NULL)
        return (NULL);

    /*
     * Skip (span) leading delimiters (s += strspn(s, delim), sort of).
     */
cont:
    c = *s++;
    for (spanp = delim; (sc = *spanp++) != 0;)
    {
        if (c == sc)
            goto cont;
    }

    if (c == 0)
    {		/* no non-delimiter characters */
        *last = NULL;
        return (NULL);
    }
    tok = s - 1;

    /*
     * Scan token (scan for delimiters: s += strcspn(s, delim), sort of).
     * Note that delim must have one NUL; we stop if we see that, too.
     */
    for (;;)
    {
        c = *s++;
        spanp = delim;
        do
        {
            if ((sc = *spanp++) == c)
            {
                if (c == 0)
                    s = NULL;
                else
                    s[-1] = '\0';
                *last = s;
                return (tok);
            }
        }
        while (sc != 0);
    }
    /* NOTREACHED */

    return NULL;
}
