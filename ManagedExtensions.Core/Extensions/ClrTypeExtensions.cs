using System.Collections.Generic;
using System.Linq;
using ManagedExtensions.Core.Dynamic;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Extensions
{
    public static class ClrTypeExtensions
    {
        public static IEnumerable<DynamicInstance> GetObjects(this ClrType type)
        {
            var heap = type.Heap;

            return heap
                .EnumerateObjectAddresses()
                .Where(address => heap.GetObjectType(address) == type)
                .Select(address => heap.GetInstance(address));
        }

        public static bool IsInheritedFrom(this ClrType type, ClrType baseType)
        {
            if (type == null)
                return false;

            if (type == baseType)
                return true;

            return IsInheritedFrom(type.BaseType, baseType);
        }

        public static bool IsDictionary(this ClrType type)
        {
            return type.Name.StartsWith("System.Collections.Generic.Dictionary<");
        }

        public static bool IsHashSet(this ClrType type)
        {
            return type.Name.StartsWith("System.Collections.Generic.HashSet<");
        }

        public static List<string> GenericTypeArguments(this ClrType type)
        {
            var typeName = type.Name;
            var firstBracket = typeName.IndexOf('<');

            if (firstBracket > 0)
            {
                var typeDelimPositions = new List<int>();

                var lastBracket = typeName.LastIndexOf('>');
                var typeArgsStr = typeName.SubstringWithEnd(firstBracket + 1, lastBracket - 1);

                int deep = 0;
                for (int i = 0; i < typeArgsStr.Length; i++)
                {
                    if (typeArgsStr[i] == '<') deep++;
                    if (typeArgsStr[i] == '>') deep--;

                    if (typeArgsStr[i] == ',' && deep == 0)
                        typeDelimPositions.Add(i);
                }

                var typeNames = typeArgsStr.Split(typeDelimPositions);

                return typeNames;
                //return typeNames
                //    .Select(t => type.Heap.GetTypeByName(t))
                //    .ToList();
            }
            return new List<string>();
        }
    }
}
