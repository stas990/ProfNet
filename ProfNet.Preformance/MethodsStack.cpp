#include "MethodsStack.h"

MethodsStack::MethodsStack(ULONG size) : _count(0), _size(0), _array(NULL)    
{    
	_array = new MethodItem*[size];
	_size = size;
}

MethodsStack::~MethodsStack()
{
	if ( _array != NULL )
	{
		delete[] _array;
		_array = NULL;
	}
}

MethodsStack::MethodsStack( const MethodsStack &source ) 
{
	_size = source._size;
	_count = source._count;
	_array = source._array;

}

ULONG MethodsStack::Count() 
{
	return _count;

}

MethodItem **GrowStack(ULONG newSize, ULONG currentSize, MethodItem **stack)
{
	// Paranoia & shut up PREFAST
	if (newSize <= currentSize)
		return stack;

	MethodItem **newStack = NULL;

	if (newSize < 0x10000000)
		newStack = new MethodItem*[newSize];

	for (ULONG i =0; i < currentSize; i++ )
	{
		newStack[i] = stack[i];
	}

	delete[] stack;
	return newStack;
}

void MethodsStack::GrowStackPush(MethodItem *item)
{
	if ( _count == _size )
	{
		_array = GrowStack(2*_count, _count, _array);
		_size = 2*_count;
	}
	_array[_count] = item;
	_count++;
} 

