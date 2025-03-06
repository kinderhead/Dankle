short test(short x, short y)
{
    return x + y;
}

short main()
{
    short ret = test(test(1, 2), test(3, 4));
    return 0;
}