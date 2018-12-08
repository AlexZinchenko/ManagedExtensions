using ManagedExtensions.Core.Dynamic;

namespace ManagedExtensions.Core.Commands
{
    public sealed class ExternalCommandNameProvider
    {
        #region Sos
        public string Do(ulong objAddress)
        {
            return $"!sos.do {objAddress:x8}";
        }

        public string DumpVC(ulong objAdress, ulong methodTableAddress)
        {
            return $"!sos.dumpvc {methodTableAddress:x8} {objAdress:x8}";
        }

        public string DumpObjectBySos(DynamicInstance obj)
        {
            if (obj.Type.IsObjectReference)
                return Do(obj.Address);
            else
                return DumpVC(obj.Address, obj.Type.MethodTable);
        }
        #endregion
    }
}
