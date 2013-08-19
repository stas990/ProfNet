#include "windows.h"
#include "winreg.h"  
#include "wincrypt.h"
#include "winbase.h"
#include "objbase.h"

#include "cor.h"
#include "corhdr.h"
#include "corhlpr.h"
#include "corerror.h"

#include "corpub.h"
#include "corprof.h"
#include "cordebug.h"
#include "SpecStrings.h"

#define ARRAY_LEN(a) (sizeof(a) / sizeof((a)[0]))
#define BYTESIZE 8

class PrfInfo
{             
    public:
        PrfInfo();
        virtual ~PrfInfo();

		void Failure( const char *message = NULL );
                   
    public:
    private:
    protected:    
        DWORD m_dwEventMask;
        ICorProfilerInfo *m_pProfilerInfo;        
        ICorProfilerInfo2 *m_pProfilerInfo2;
        ICorProfilerInfo3 *m_pProfilerInfo3;
}; 
