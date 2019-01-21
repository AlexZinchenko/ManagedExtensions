using System;
using System.Globalization;
using System.Runtime.InteropServices;
using ManagedExtensions.Core;
using ManagedExtensions.Core.Commands;
using ManagedExtensions.Core.Verifiers;

namespace ManagedExtensions.CommonCommands.DependencyProperties
{
    [CommandGroup(Id = CommandGroupId.Wpf)]
    public sealed class DumpDepPropertyCommand : BaseCommand
    {
        public DumpDepPropertyCommand(ICommandsHost debugger) : base(debugger)
        {
        }

        [DllExport, CommandMethod]
        public static void dumpdp(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<DumpDepPropertyCommand>(client, c => c.DumpDependencyProperty(args));
        }

        public void DumpDependencyProperty(string args)
        {
            var argsList = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
  
            if (argsList.Length == 2)
            {
                var objAddress = ulong.Parse(argsList[0], NumberStyles.HexNumber);
                var propertyName = argsList[1];

                if (!VerifyHelpers.VerifyObjAddress(objAddress, Heap, Output))
                    return;

                var depPropertiesProvider = DepPropertiesProvider.Get(objAddress, Heap);
                var propInfo = depPropertiesProvider.GetPropertyInfo(propertyName);

                Output.WriteLine("Prop name:\t{0}", propInfo.Name);
                Output.WriteLine("Value info:");
                Output.WriteLine("-----------");
                Output.Execute(ExternalCommandNames.DumpStructOrObject(propInfo.Value));
            }
            else
            {
                Output.WriteLine("Invalid arguments count. Usage:");
                WriteHelp();
            }
        }

        public string GetCallString(ulong objAddress, string propName)
        {
            return $"!{nameof(dumpdp)} {objAddress:x8} {propName}";
        }

        public override void WriteHelp()
        {
            Output.WriteLine("-------------------------------------------------------------------------------");
            Output.WriteLine($"!{nameof(dumpdp)} <object address> <dependency property name>");
            Output.WriteLine();
            Output.WriteLine("This command allows you to examine value of specified dependency property of specified dependency object");
            Output.WriteLine("It searches property value and then uses commands of SOS to dump value");
            Output.WriteLine($"Usage example:");
            Output.WriteLine(
@"
    0:014> !{0} 17e5ad18 Padding
    Prop name:	Padding
    Value info:
    -----------
    Name:        System.Windows.Thickness
    MethodTable: 112e2b68
    EEClass:     091e52a0
    Size:        40(0x28) bytes
    File:        C:\Windows\Microsoft.Net\assembly\GAC_MSIL\PresentationFramework\v4.0_4.0.0.0__31bf3856ad364e35\PresentationFramework.dll
    Fields:
          MT    Field   Offset                 Type VT     Attr    Value Name
    0af9dba0  4004373        0        System.Double  1 instance 0.000000 _Left
    0af9dba0  4004374        8        System.Double  1 instance 0.000000 _Top
    0af9dba0  4004375       10        System.Double  1 instance 0.000000 _Right
    0af9dba0  4004376       18        System.Double  1 instance 0.000000 _Bottom", nameof(dumpdp));
        }
    }
}
