#include "ProfilerInfo.h"
#include "Logger.h"

PrfInfo::PrfInfo() :         
    m_pProfilerInfo( NULL ),
    m_pProfilerInfo2( NULL ),
    m_pProfilerInfo3( NULL ),
    m_dwEventMask( 0 )
{

}

PrfInfo::~PrfInfo()
{
    if ( m_pProfilerInfo != NULL )
        m_pProfilerInfo->Release();
       
    if ( m_pProfilerInfo2 != NULL )
        m_pProfilerInfo2->Release();

    if ( m_pProfilerInfo3 != NULL )
        m_pProfilerInfo3->Release();
}

void PrfInfo::Failure( const char *message )
{
    if ( message == NULL )     
        message = "**** SEVERE FAILURE: TURNING OFF APPLICABLE PROFILING EVENTS ****";  
        
    //m_pProfilerInfo->SetEventMask( (m_dwEventMask & (DWORD)COR_PRF_MONITOR_IMMUTABLE) );    
                        
}

#define ARRAY_LEN(a) (sizeof(a) / sizeof((a)[0]))
