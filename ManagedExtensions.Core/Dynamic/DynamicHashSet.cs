using ManagedExtensions.Core.Extensions;
using Microsoft.Diagnostics.Runtime;
using System.Collections;
using System.Collections.Generic;

namespace ManagedExtensions.Core.Dynamic
{
    public sealed class DynamicHashSet : DynamicInstance, IEnumerable<DynamicInstance>
    {
        public DynamicHashSet(ulong address, ClrHeap heap) : base(address, heap)
        {
            Count = (int)AsDynamic.m_count;

            ElementTypeName = Type.GenericTypeArguments()[0];
        }

        public int Count { get; }
        public string ElementTypeName { get; }

        public IEnumerator<DynamicInstance> GetEnumerator()
        {
            return new HashSetEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class HashSetEnumerator : IEnumerator<DynamicInstance>
        {
            public HashSetEnumerator(DynamicHashSet hashSet)
            {
                _hashSet = hashSet;
                _maxIndexOfWhereverAddedElement = (int)hashSet.AsDynamic.m_lastIndex - 1;
                _elements = hashSet.AsDynamic.m_slots;
            }

            public DynamicInstance Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (_elements != null)
                {
                    while (_index <= _maxIndexOfWhereverAddedElement)
                    {
                        var element = _elements[_index].AsDynamic;
                        if ((int)element.hashCode >= 0)
                        {
                            Current = element.value;
                            _index++;
                            return true;
                        }
                        _index++;
                    }
                }

                Current = null;
                return false;
            }

            public void Reset()
            {
                _index = 0;
                Current = null;
            }

            private readonly DynamicHashSet _hashSet;
            private readonly int _maxIndexOfWhereverAddedElement;
            private readonly DynamicArray _elements;
            private int _index;
        }
    }
}
