#ifndef _STDINT_H
#define _STDINT_H

// The C89 standard doesn't need this I think, so it's pretty barebones

typedef unsigned char uint8_t;
typedef signed char int8_t;
typedef unsigned short uint16_t;
typedef signed short int16_t;
typedef unsigned int uint32_t;
typedef signed int int32_t;
typedef unsigned long uint64_t;
typedef signed long int64_t;

typedef signed long intmax_t;
typedef unsigned int uintptr_t;

#endif