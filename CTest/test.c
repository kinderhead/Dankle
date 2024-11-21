void write(char c)
{
    *((unsigned char*)(0xFFFFFFF0u)) = c;
}

void println(const char* str)
{
    while (*str != 0)
    {
        write(*str);
        str++;
    }
}

int main()
{
    println("Hello C!\n");

    

    return 0;
}
