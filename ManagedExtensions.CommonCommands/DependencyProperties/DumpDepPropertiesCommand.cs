using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedExtensions.Core;
using ManagedExtensions.Core.Commands;
using ManagedExtensions.Core.Out;
using ManagedExtensions.Core.Out.Primitives;
using ManagedExtensions.Core.Out.TableOutput;
using ManagedExtensions.Core.Verifiers;

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

            if (!VerifyHelpers.VerifyObjAddress(objAddress, Heap, Output))
                return;

            var depPropertiesProvider = DepPropertiesProvider.Get(objAddress, Heap);

            var columns = new List<Column>
            {
                new Column("Type", minWidth: 30),
                new Column("Value"),
                new Column("Name")
            };

            using (var table = new Table(Output, columns))
            {
                foreach (var propertyInfo in depPropertiesProvider.Properties.OrderBy(p => p.Name))
                {
                    var row = table.CreateRow();
                    row.Add(propertyInfo.Type.Name);
                    row.Add(GetChunk(objAddress, propertyInfo));
                    row.Add(propertyInfo.Name);

                    table.AddRow(row);
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

 | Type                                           | Value            | Name                        | 
 | ---------------------------------------------- | ---------------- | --------------------------- | 
 | System.Double                                  | 0                | ActualHeight                | 
 | System.Double                                  | 0                | ActualWidth                 | 
 | System.Boolean                                 | False            | AllowDrop                   | 
 | System.Boolean                                 | False            | AreAnyTouchesCaptured       | 
 | System.Boolean                                 | False            | AreAnyTouchesCapturedWithin | 
 | System.Boolean                                 | False            | AreAnyTouchesDirectlyOver   | 
 | System.Boolean                                 | False            | AreAnyTouchesOver           | 
 | System.Windows.Media.Brush                     | NULL             | Background                  | 
 | System.Double                                  | 33.7266666666667 | BaselineOffset              | 
 | System.Windows.Data.BindingGroup               | NULL             | BindingGroup                | 
 | System.Windows.Media.Effects.BitmapEffect      | NULL             | BitmapEffect                | 
 | System.Windows.Media.Effects.BitmapEffectInput | NULL             | BitmapEffectInput           | 
 | System.Windows.Media.CacheMode                 | NULL             | CacheMode                   | 
 | System.Windows.Media.Geometry                  | NULL             | Clip                        | 
 | System.Boolean                                 | False            | ClipToBounds                | 
 | System.Windows.Controls.ContextMenu            | NULL             | ContextMenu                 | 
 | System.Windows.Input.Cursor                    | NULL             | Cursor                      | 
 | System.Object                                  | NULL             | DataContext                 | 
 | System.Windows.Media.Effects.Effect            | NULL             | Effect                      | 
 | System.Windows.FlowDirection                   | LeftToRight      | FlowDirection               | 
 | System.Boolean                                 | False            | Focusable                   | 
 | System.Windows.Style                           | 0b5fce38         | FocusVisualStyle            | 
 | System.Windows.Media.FontFamily                | 0b607bd8         | FontFamily                  | 
 | System.Double                                  | 12               | FontSize                    | 
 | System.Windows.FontStretch                     | 0b607f24         | FontStretch                 | 
 | System.Windows.FontStyle                       | 0b607e38         | FontStyle                   | 
 | System.Windows.FontWeight                      | 0b607ea0         | FontWeight                  | 
 | System.Boolean                                 | False            | ForceCursor                 | 
 | System.Windows.Media.Brush                     | 0b608500         | Foreground                  | 
 | System.Double                                  | NaN              | Height                      | 
 | System.Windows.HorizontalAlignment             | Stretch          | HorizontalAlignment         | 
 | System.Windows.Input.InputScope                | NULL             | InputScope                  | 
 | System.Boolean                                 | True             | IsEnabled                   | 
 | System.Boolean                                 | False            | IsFocused                   | 
 | System.Boolean                                 | True             | IsHitTestVisible            | 
 | System.Boolean                                 | False            | IsHyphenationEnabled        | 
 | System.Boolean                                 | False            | IsKeyboardFocused           | 
 | System.Boolean                                 | False            | IsKeyboardFocusWithin       | 
 | System.Boolean                                 | False            | IsManipulationEnabled       | 
 | System.Boolean                                 | False            | IsMouseCaptured             | 
 | System.Boolean                                 | False            | IsMouseCaptureWithin        | 
 | System.Boolean                                 | False            | IsMouseDirectlyOver         | 
 | System.Boolean                                 | False            | IsMouseOver                 | 
 | System.Boolean                                 | False            | IsStylusCaptured            | 
 | System.Boolean                                 | False            | IsStylusCaptureWithin       | 
 | System.Boolean                                 | False            | IsStylusDirectlyOver        | 
 | System.Boolean                                 | False            | IsStylusOver                | 
 | System.Boolean                                 | False            | IsVisible                   | 
 | System.Windows.Markup.XmlLanguage              | 0b5fb788         | Language                    | 
 | System.Windows.Media.Transform                 | 0b5fc0b8         | LayoutTransform             | 
 | System.Double                                  | NaN              | LineHeight                  | 
 | System.Windows.LineStackingStrategy            | MaxHeight        | LineStackingStrategy        | 
 | System.Windows.Thickness                       | 0b5fcb4c         | Margin                      | 
 | System.Double                                  | Infinity         | MaxHeight                   | 
 | System.Double                                  | Infinity         | MaxWidth                    | 
 | System.Double                                  | 0                | MinHeight                   | 
 | System.Double                                  | 0                | MinWidth                    | 
 | System.String                                  | """"               | Name                        | 
 | System.Double                                  | 1                | Opacity                     | 
 | System.Windows.Media.Brush                     | NULL             | OpacityMask                 | 
 | System.Boolean                                 | False            | OverridesDefaultStyle       | 
 | System.Windows.Thickness                       | 0b728420         | Padding                     | 
 | System.Windows.Media.Transform                 | 0b5fc0b8         | RenderTransform             | 
 | System.Windows.Point                           | 0b6020a8         | RenderTransformOrigin       | 
 | System.Boolean                                 | False            | SnapsToDevicePixels         | 
 | System.Windows.Style                           | NULL             | Style                       | 
 | System.Object                                  | NULL             | Tag                         | 
 | System.String                                  | """"               | Text                        | 
 | System.Windows.TextAlignment                   | Left             | TextAlignment               | 
 | System.Windows.TextDecorationCollection        | 0b728000         | TextDecorations             | 
 | System.Windows.Media.TextEffectCollection      | 0b608aa8         | TextEffects                 | 
 | System.Windows.TextTrimming                    | None             | TextTrimming                | 
 | System.Windows.TextWrapping                    | NoWrap           | TextWrapping                | 
 | System.Object                                  | NULL             | ToolTip                     | 
 | System.String                                  | """"               | Uid                         | 
 | System.Boolean                                 | False            | UseLayoutRounding           | 
 | System.Windows.VerticalAlignment               | Stretch          | VerticalAlignment           | 
 | System.Windows.Visibility                      | 0                | Visibility                  | 
 | System.Double                                  | NaN              | Width                       |", nameof(dumpdps));
            }
        }

        private Chunk GetChunk(ulong objAddress, DepPropertyInfo propInfo)
        {
            return ChunkFactory.CreateTextOrLink(
                propInfo.Value, 
                () => Commands.DumpDpCommand().GetCallString(objAddress, propInfo.Name));
        }
    }
}
