using System;
using System.Collections.Generic;
using System.Linq;
using ManagedExtensions.Core.Dynamic;
using ManagedExtensions.Core.Exceptions;
using ManagedExtensions.Core.Extensions;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.CommonCommands.DependencyProperties
{
    public sealed class DepPropertiesProvider
    {
        public static DepPropertiesProvider Get(ulong objAddress, ClrHeap heap)
        {
            if (_cachedProvider != null && _cachedProvider.ObjAddress == objAddress)
                return _cachedProvider;

            return (_cachedProvider = new DepPropertiesProvider(objAddress, heap));
        }

        private DepPropertiesProvider(ulong objAddress, ClrHeap heap)
        {
            _targetObject = heap.GetInstance(objAddress);

            _effectiveValues = _targetObject.AsDynamic._effectiveValues;

            _depPropertyType = heap.GetTypeByName(DependencyPropertyTypeName);
        }

        public ulong ObjAddress => _targetObject.Address;

        public DepPropertyInfo GetPropertyInfo(string propertyName)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);

            if (property != null)
                return property;

            throw new CommandException($"property {propertyName} wasn't found");
        }

        public List<DepPropertyInfo> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = GetProperties();

                return _properties;
            }
        }

        private List<DepPropertyInfo> GetProperties()
        {
            return _targetObject
                    .GetStaticFields(sf => sf.IsPublic && sf.Type == _depPropertyType, includeParents: true)
                    .Select(depProperty =>
                    {
                        ClrType propertyType = depProperty.AsDynamic._propertyType;
                        string propertyName = depProperty.AsDynamic._name;

                        var propertyValue = GetPropertyValue(depProperty);

                        return new DepPropertyInfo(propertyType, propertyValue, propertyName);
                    })
                    .ToList();
        }

        private DynamicInstance GetPropertyValue(DynamicInstance depProperty)
        {
            int packedData = depProperty.AsDynamic._packedData;
            var globalIndex = (short)(packedData & 0xFFFF);

            var effectiveValue = _effectiveValues.FirstOrDefault(ev => (short)ev.AsDynamic._propertyIndex == globalIndex);
            if (effectiveValue != null)
            {
                DynamicInstance internalValue = effectiveValue.AsDynamic._value;
                if (internalValue.Type.Name == ModifiedValueTypeName)
                {
                    short fullValueSource = effectiveValue.AsDynamic._source;
                    if (IsCoerced(fullValueSource))
                        return internalValue.AsDynamic._coercedValue;
                    if (IsAnimated(fullValueSource))
                        return internalValue.AsDynamic._animatedValue;
                    if (IsExpression(fullValueSource))
                        return internalValue.AsDynamic._expressionValue;

                    throw new InvalidOperationException("Can't get value of property");
                }
            }

            DynamicInstance defaultValue = depProperty.AsDynamic._defaultMetadata._defaultValue;

            return defaultValue;
        }
        
        private bool IsCoerced(short fullValueSource)
        {
            return (fullValueSource & (short)FullValueSource.IsCoerced) != 0;
        }

        private bool IsAnimated(short fullValueSource)
        {
            return (fullValueSource & (short)FullValueSource.IsAnimated) != 0;
        }

        private bool IsExpression(short fullValueSource)
        {
            return (fullValueSource & (short)FullValueSource.IsExpression) != 0;
        }

        private DynamicInstance _targetObject;
        private DynamicArray _effectiveValues;
        private List<DepPropertyInfo> _properties;
        private ClrType _depPropertyType;

        private readonly string DependencyPropertyTypeName = "System.Windows.DependencyProperty";
        private readonly string ModifiedValueTypeName = "System.Windows.ModifiedValue";

        private static DepPropertiesProvider _cachedProvider;

        private enum FullValueSource: short
        {
            ValueSourceMask     = 0x000F,
            ModifiersMask       = 0x0070,
            IsExpression        = 0x0010,
            IsAnimated          = 0x0020,
            IsCoerced           = 0x0040,
            IsPotentiallyADeferredReference = 0x0080,
            HasExpressionMarker = 0x0100,
            IsCoercedWithCurrentValue = 0x200,
        }
    }
}
