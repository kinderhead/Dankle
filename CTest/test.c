long test(long x, long y)
{
    return x + y;
}

short main()
{
    long ret = test(test(1, 2), test(3, 4));
    return 0;
}