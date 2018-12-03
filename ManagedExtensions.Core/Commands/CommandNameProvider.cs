namespace ManagedExtensions.Core.Commands
{
    public sealed class ExternalCommandNameProvider
    {
        #region Sos
        public string Do(ulong objAddress)
        {
            return $"!do {objAddress:x8}";
        }

        public string DumpVC(ulong objAdress, ulong methodTableAddress)
        {
            return $"!dumpvc {methodTableAddress:x8} {objAdress:x8}";
        } 
        #endregion
    }
}
