using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedExtensions.Core;
using ManagedExtensions.Core.Commands;
using ManagedExtensions.Core.Out;

namespace ManagedExtensions.CommonCommands.DependencyProperties
{
    [CommandGroup(Id = CommandGroupId.Wpf)]
    public sealed class DumpDepPropertiesCommand : BaseCommand
    {
        public DumpDepPropertiesCommand(ICommandsHost debugger) : base(debugger)
        {
        }

        [DllExport, CommandMethod]
        public static void dumpdps(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<DumpDepPropertiesCommand>(client, c => c.DumpDependencyProperties(args));
        }

        public void DumpDependencyProperties(string args)
        {
            var objAddress = ulong.Parse(args, NumberStyles.HexNumber);

            var depPropertiesProvider = DepPropertiesProvider.Get(objAddress, Heap);

            var columns = new Column[]
            {
                new Column("Type", 40),
                new Column("Value", 20),
                new Column("Name", 50)
            };

            using (var table = new Table(Output, columns))
            {
                foreach (var propertyInfo in depPropertiesProvider.Properties.OrderBy(p => p.Name))
                {
                    var row = table.CreateRow();
                    row.Add(propertyInfo.Type.Name);
                    row.Add(GetPropValue(objAddress, propertyInfo));
                    row.Add(propertyInfo.Name);

                    table.WriteRow(row);
                }
            }
        }

        public override void WriteHelp()
        {
            {
                Output.WriteLine("-------------------------------------------------------------------------------");
                Output.WriteLine($"!{nameof(dumpdps)} <object address>");
                Output.WriteLine();
                Output.WriteLine("This command allows you to examine all dependency properties of specified dependency object");
                Output.WriteLine("If property value is null or it is primitive (int, bool, enum etc) then value is displayed in table row. Otherwise table has address of property value");
                Output.WriteLine($"Usage example:");
                Output.WriteLine(
    @"
    0:014> !{0} 17e5ad18
                                        Type |                Value | Name
    --------------------------------------------------------------------------------------------------------------------
                               System.Double |                    0 | ActualHeight
                               System.Double |                    0 | ActualWidth
                              System.Boolean |                False | AllowDrop
                              System.Boolean |                False | AreAnyTouchesCaptured
                  System.Windows.Media.Brush |                 NULL | Background
                               System.Double |     33.7266666666667 | BaselineOffset
            System.Windows.Data.BindingGroup |                 NULL | BindingGroup
                               System.Object |                 NULL | DataContext
         System.Windows.Media.Effects.Effect |                 NULL | Effect
                System.Windows.FlowDirection |          LeftToRight | FlowDirection
                              System.Boolean |                False | Focusable
                        System.Windows.Style |            190828088 | FocusVisualStyle
             System.Windows.Media.FontFamily |            190872536 | FontFamily
                               System.Double |                   12 | FontSize
                  System.Windows.FontStretch |            190873380 | FontStretch
                    System.Windows.FontStyle |            190873144 | FontStyle
                   System.Windows.FontWeight |            190873248 | FontWeight
                              System.Boolean |                False | ForceCursor
                              System.Boolean |                 True | IsHitTestVisible", nameof(dumpdps));
            }
        }

        private object GetPropValue(ulong objAddress, DepPropertyInfo propInfo)
        {
            var propValue = propInfo.Value;
            if (propValue.IsNull)
            {
                return "NULL";
            }

            var displayedValue = propValue.DisplayedPrimitiveValue;
            var propType = propValue.Type;

            if (propType.IsPrimitive)
            {
                return displayedValue;
            }

            var dumpDpCommand = Commands.DumpDpCommand().GetCallString(objAddress, propInfo.Name);
            return new Link(propInfo.Value.Address, dumpDpCommand);
        }
    }
}
