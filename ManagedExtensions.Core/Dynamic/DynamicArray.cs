using System.Collections;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Dynamic
{
    public class DynamicArray : DynamicInstance, IEnumerable<DynamicInstance>
    {
        public DynamicArray(ulong address, ClrHeap heap)
            : base(address, heap)
        {
            Length = Type.GetArrayLength(Address);
        }

        public int Length { get; private set; }

        public IEnumerator<DynamicInstance> GetEnumerator()
        {
            return new ArrayEnumerator(Type, Address, Heap);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class ArrayEnumerator : IEnumerator<DynamicInstance>
        {
            public ArrayEnumerator(ClrType type, ulong address, ClrHeap heap)
            {
                _arrayType = type;
                _address = address;
                _heap = heap;

                Reset();
            }

            public DynamicInstance Current
            {
                get
                {
                    var elementAddress = _arrayType.GetArrayElementAddress(_address, _currentIndex);

                    return new DynamicInstance(elementAddress, _arrayType.ComponentType, _heap);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return (++_currentIndex < _arrayType.GetArrayLength(_address));
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public void Dispose()
            {
            }

            private int _currentIndex;
            private ClrHeap _heap;
            private ClrType _arrayType;
            private ulong _address;
        }
    }
}
