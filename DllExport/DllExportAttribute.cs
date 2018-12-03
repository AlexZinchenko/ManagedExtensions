using System;
using System.Runtime.InteropServices;

namespace ManagedExtensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [Serializable]
    public sealed class DllExportAttribute : Attribute
    {
        public CallingConvention CallingConvention { get; set; } = CallingConvention.Cdecl;

        public string ExportName { get; set; }

        public DllExportAttribute(string function, CallingConvention convention)
        {
        }

        public DllExportAttribute(string function)
        {
        }

        public DllExportAttribute(CallingConvention convention)
        {
        }

        public DllExportAttribute()
        {
        }
    }
}