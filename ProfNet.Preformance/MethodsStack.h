#include "MethodItem.h"

class MethodsStack
{
public:

	MethodsStack( ULONG size );
	MethodsStack( const MethodsStack &source );
	~MethodsStack();

	__forceinline void Push( MethodItem *item )
	{
		if ( _count < _size )
		{
			_array[_count] = item;
			_count++;
		}
		else
			GrowStackPush( item );
	}

	__forceinline void Pop()
	{
		if (_count != 0 )
			_count--;
	}

	void GrowStackPush(MethodItem *item);
	__forceinline MethodItem *Top()
	{        

		if (_count == 0)
			return NULL;

		else
			return _array[_count-1];

	}
	ULONG Count();
	MethodItem **_array;

private:

	ULONG _count;
	ULONG _size;
};