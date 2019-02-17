using Microsoft.Diagnostics.Runtime;
using System.Collections;
using System.Collections.Generic;
using ManagedExtensions.Core.Extensions;

namespace ManagedExtensions.Core.Dynamic
{
    public sealed class DynamicDictionary : DynamicInstance, IEnumerable<KeyValuePair<DynamicInstance, DynamicInstance>>
    {
        public DynamicDictionary(ulong address, ClrHeap heap) : base(address, heap)
        {
            Count = (int)AsDynamic.count - (int)AsDynamic.freeCount;
            
            var genericTypeNames = Type.GenericTypeArguments();

            KeyTypeName = genericTypeNames[0];
            ValueTypeName = genericTypeNames[1];
        }

        public int Count { get; }
        public string KeyTypeName { get; }
        public string ValueTypeName { get; }

        public IEnumerable<DynamicInstance> Keys
        {
            get
            {
                foreach (var pair in this)
                {
                    yield return pair.Key;
                }
            }
        }

        public IEnumerable<DynamicInstance> Values
        {
            get
            {
                foreach (var pair in this)
                {
                    yield return pair.Value;
                }
            }
        }

        public IEnumerator<KeyValuePair<DynamicInstance, DynamicInstance>> GetEnumerator()
        {
            return new DictionaryEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class DictionaryEnumerator : IEnumerator<KeyValuePair<DynamicInstance, DynamicInstance>>
        {
            public DictionaryEnumerator(DynamicDictionary dictionary)
            {
                _dictionary = dictionary;
                _maxIndexOfWhereverAddedEntry = (int)dictionary.AsDynamic.count - 1;
                _entries = _dictionary.AsDynamic.entries;
                Reset();
            }

            public KeyValuePair<DynamicInstance, DynamicInstance> Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_entries != null)
                {
                    while (_currentIndex <= _maxIndexOfWhereverAddedEntry)
                    {
                        var dynamicEntry = _entries[_currentIndex++].AsDynamic;

                        int hashCode = dynamicEntry.hashCode;
                        if (hashCode > -1)
                        {
                            Current = new KeyValuePair<DynamicInstance, DynamicInstance>(dynamicEntry.key, dynamicEntry.value);
                            return true;
                        }
                    }
                }

                Current = default(KeyValuePair<DynamicInstance, DynamicInstance>);
                return false;
            }

            public void Reset()
            {
                _currentIndex = 0;
            }

            public void Dispose()
            {
            }

            private readonly DynamicDictionary _dictionary;
            private readonly DynamicArray _entries;
            private int _currentIndex;
            private int _maxIndexOfWhereverAddedEntry;
        }
    }
}
