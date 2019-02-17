using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using ManagedExtensions.Core.Extensions;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Dynamic
{
    public class DynamicInstance : DynamicObject
    {
        public DynamicInstance(ulong address, ClrHeap heap)
        {
            Address = address;
            Heap = heap;
            Type = heap.GetObjectType(Address);

            Init();
        }

        public DynamicInstance(ulong address, ClrType type, ClrHeap heap)
        {
            Address = address;
            Heap = heap;

            if (!type.IsValueClass && Address != 0)
                Type = heap.GetObjectType(Address);
            else
                Type = type;

            Init();
        }

        public ClrType Type { get; private set; }
        public ulong Address { get; private set; }
        public string HexAddress { get { return $"{Address:x8}"; } }
        public dynamic AsDynamic => this;
        public object PrimitiveValue { get; private set; }
        public string DisplayedPrimitiveValue { get; private set; }
        public bool IsNull => Address == 0 && Type.IsObjectReference;
        public bool IsDictionary => Type.IsDictionary();
        public bool IsHashSet => Type.IsHashSet();
        public bool IsString => Type.IsString;

        public IEnumerable<DynamicInstance> GetStaticFields(Func<ClrStaticField, bool> fieldsFilter, bool includeParents = false)
        {
            var appDomain = Heap.Runtime.AppDomains[0];

            IEnumerable<ClrStaticField> fields = includeParents ? GetAllStaticFields(Type) : Type.StaticFields;

            return fields.Where(fieldsFilter).Select(sf => FromField(sf.GetValue(appDomain, convertStrings: false), sf, Heap));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var field = Type.GetFieldByName(binder.Name);
            if (field != null)
            {
                var isObjReference = field.Type.IsObjectReference;
                var isPrimitive = field.Type.IsPrimitive;
                object fieldValue;

                if (isObjReference || isPrimitive)
                    fieldValue = field.GetValue(Address, Type.IsValueClass, convertStrings: false);
                else
                    fieldValue = field.GetAddress(Address, Type.IsValueClass);
                
                result = FromField(fieldValue, field, Heap);

                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (IsNull)
            {
                result = null;
                return true;
            }

            if (binder.ReturnType == typeof(ClrType) && Type.Name == "System.RuntimeType")
            {
                result = ConvertToClrType();
                return true;
            }

            if (binder.ReturnType == typeof(DynamicArray) && Type.IsArray)
            {
                result = new DynamicArray(Address, Heap);
                return true;
            }

            if (binder.ReturnType == typeof(DynamicDictionary) && IsDictionary)
            {
                result = new DynamicDictionary(Address, Heap);
                return true;
            }

            if (binder.ReturnType == typeof(DynamicHashSet) && IsHashSet)
            {
                result = new DynamicHashSet(Address, Heap);
                return true;
            }

            if (binder.ReturnType.IsPrimitive || binder.ReturnType == typeof(string) && PrimitiveValue != null)
            {
                result = PrimitiveValue;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        protected ClrHeap Heap { get; private set; }

        private ClrType ConvertToClrType()
        {
            long methodTableAddr = AsDynamic.m_handle;
            var clrType = Heap.GetTypeByMethodTable((ulong)methodTableAddr);

            return clrType;
        }

        private IEnumerable<ClrStaticField> GetAllStaticFields(ClrType type)
        {
            if (type == null)
                return new List<ClrStaticField>();

            return GetAllStaticFields(type.BaseType).Concat(type.StaticFields);
        }

        private DynamicInstance(object primitiveValue, ClrType type, ClrHeap heap)
        {
            Type = type;
            Heap = heap;
            InitPrimitiveValue(primitiveValue);
        }

        private void Init()
        {
            InitPrimitiveValue();
        }

        private void InitPrimitiveValue(object sourcePrimitiveValue = null)
        {
            if (IsNull)
            {
                PrimitiveValue = null;
                DisplayedPrimitiveValue = "NULL";
                return;
            }

            PrimitiveValue = sourcePrimitiveValue;

            if (sourcePrimitiveValue == null
                && Type.HasSimpleValue
                && (!Type.IsObjectReference || IsString)
                && Address != 0)
            {
                PrimitiveValue = Type.GetValue(Address);
            }

            if (PrimitiveValue != null)
            {
                if (!Type.IsEnum)
                {
                    DisplayedPrimitiveValue = string.Format(CultureInfo.InvariantCulture, "{0}", PrimitiveValue);
                }
                else
                {
                    DisplayedPrimitiveValue = Type.GetEnumName(PrimitiveValue);
                    if (DisplayedPrimitiveValue == null)
                        DisplayedPrimitiveValue = PrimitiveValue.ToString();
                }
            }
        }

        private static DynamicInstance FromField(object value, ClrField field, ClrHeap heap)
        {
            if (field.IsPrimitive)
                return new DynamicInstance(value, field.Type, heap);

            return new DynamicInstance((ulong)value, field.Type, heap);
        }
    }
}