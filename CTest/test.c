void print(const char* str)
{
    while (*str != 0)
    {
        *((char*) 0xFFFFFFF0) = *(str++);
    }
}

short main()
{
    print("We do a little testing");

    return 0;
}
