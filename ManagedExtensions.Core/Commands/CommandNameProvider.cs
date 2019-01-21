using ManagedExtensions.Core.Dynamic;

namespace ManagedExtensions.Core.Commands
{
    public sealed class ExternalCommandNameProvider
    {
        #region Sos
        public string DumpObj(ulong objAddress, bool bySos = false)
        {
            var alias = bySos ? "sos." : string.Empty;
            return $"!{alias}do {objAddress:x8}";
        }

        public string DumpVC(ulong objAdress, ulong methodTableAddress, bool bySos = false)
        {
            var alias = bySos ? "sos." : string.Empty;
            return $"!{alias}dumpvc {methodTableAddress:x8} {objAdress:x8}";
        }

        public string DumpStructOrObject(DynamicInstance obj, bool bySos = false)
        {
            if (obj.Type.IsObjectReference)
                return DumpObj(obj.Address);
            else
                return DumpVC(obj.Address, obj.Type.MethodTable);
        }
        #endregion
    }
}
