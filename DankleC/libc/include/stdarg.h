#ifndef _STDARG_H
#define _STDARG_H

#define va_start(ap, argN) ap = ((va_list)&(argN)) + (sizeof argN)
#define va_arg(ap, type) ((type*)(ap = ap + sizeof(type)))[-1]

typedef void* va_list;

#endif
