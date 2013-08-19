#ifndef PROFILER_GLOBAL_H
#define PROFILER_GLOBAL_H

#include <string>
#include <vector>
#include <map>
#include <list>

#ifdef PROFILER_LIB
# define PROFILER_EXPORT Q_DECL_EXPORT
#else
# define PROFILER_EXPORT Q_DECL_IMPORT
#endif

#endif // PROFILER_GLOBAL_H