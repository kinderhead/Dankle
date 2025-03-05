short test(short x, short y)
{
    int _ = 0;
    return x + y;
}

short main()
{
    int _ = 0;
    short x = test(32766, 1);
    return 0;
}
