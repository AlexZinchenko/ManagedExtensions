using System;
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

        public DynamicInstance this[int index]
        {
            get
            {
                if (index >= Length || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, "index out of range");

                var elementAddress = Type.GetArrayElementAddress(Address, index);

                return new DynamicInstance(elementAddress, Type.ComponentType, Heap);
            }
        }
        public IEnumerator<DynamicInstance> GetEnumerator()
        {
            return new ArrayEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class ArrayEnumerator : IEnumerator<DynamicInstance>
        {
            public ArrayEnumerator(DynamicArray dArray)
            {
                Reset();
                _dArray = dArray;
            }

            public DynamicInstance Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_currentIndex < _dArray.Length)
                {
                    Current = _dArray[_currentIndex];
                    _currentIndex++;

                    return true;
                }

                Current = null;
                return false;
            }

            public void Reset()
            {
                _currentIndex = 0;
            }

            public void Dispose()
            {
            }

            private int _currentIndex;
            private readonly DynamicArray _dArray;
        }
    }
}
