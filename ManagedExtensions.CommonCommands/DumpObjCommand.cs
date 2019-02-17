using ManagedExtensions.Core;
using ManagedExtensions.Core.Commands;
using ManagedExtensions.Core.Dynamic;
using ManagedExtensions.Core.Extensions;
using ManagedExtensions.Core.Out;
using ManagedExtensions.Core.Out.Primitives;
using ManagedExtensions.Core.Out.TableOutput;
using ManagedExtensions.Core.Verifiers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace ManagedExtensions.CommonCommands
{
    [CommandGroup(Id = CommandGroupId.Common)]
    public sealed class DumpObjCommand : ManagedCommand
    {
        public DumpObjCommand(IManagedCommandsHost debugger)
            : base(debugger)
        {
        }

        [DllExport, CommandMethod]
        public static void DumpObj(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<DumpObjCommand>(client, c => c.DumpObject(args));
        }

        [DllExport, CommandMethod]
        public static void dumpobj(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<DumpObjCommand>(client, c => c.DumpObject(args));
        }

        [DllExport(ExportName = "do"), CommandMethod]
        public static void _do_(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<DumpObjCommand>(client, c => c.DumpObject(args));
        }

        private void DumpObject(string args)
        {
            var objAddressArg = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
            var objAddress = ulong.Parse(objAddressArg, NumberStyles.HexNumber);
            if (!VerifyHelpers.VerifyObjAddress(objAddress, Heap, Output))
                return;

            var instance = Heap.GetInstance(objAddress);

            Output.Execute(ExternalCommandNames.DumpObj(objAddress, bySos: true));

            if (instance.IsDictionary)
            {
                DumpDictionary(instance);
            }
            else if (instance.IsHashSet)
            {
                DumpHashSet(instance);
            }
        }

        private void DumpHashSet(DynamicInstance instance)
        {
            DynamicHashSet hashSet = instance.AsDynamic;

            if (hashSet.Count == 0)
                return;

            Output.WriteLine($"\nElements [{hashSet.Count}]:");

            var columns = new List<Column>();
            var displayElementType = hashSet.ToList().Any(v => !v.IsNull && v.Type.Name != hashSet.ElementTypeName);
            if (displayElementType)
            {
                columns.Add(new Column("ElementType", minWidth: 10) { Align = Align.Left });
            }

            columns.Add(new Column("Element") { Align = Align.Left });
            
            using (var table = new Table(Output, columns))
            {
                foreach (var element in hashSet)
                {
                    var row = table.CreateRow();

                    if (displayElementType)
                    {
                        row.Add(element.Type.Name);
                    }
                    row.Add(GetChunk(element));
                    
                    table.AddRow(row);
                }
            }
        }

        private void DumpDictionary(DynamicInstance instance)
        {
            DynamicDictionary dict = instance.AsDynamic;

            if (dict.Count == 0)
                return;

            Output.WriteLine($"\nItems [{dict.Count}]:");

            var columns = new List<Column>();
            var displayKeyType = dict.Keys.Any(k => !k.IsNull && k.Type.Name != dict.KeyTypeName);
            var displayValueType = dict.Values.Any(k => !k.IsNull && k.Type.Name != dict.ValueTypeName);

            if (displayKeyType)
            {
                columns.Add(new Column("KeyType", minWidth: 10) { Align = Align.Left });
            }

            columns.Add(new Column("Key") { Align = Align.Right });
            columns.Add(new Column("Value") { Align = Align.Left });

            if (displayValueType)
            {
                columns.Add(new Column("ValueType", minWidth: 10) { Align = Align.Left });
            }

            using (var table = new Table(Output, columns))
            {
                foreach (var pair in dict)
                {
                    var row = table.CreateRow();

                    if (displayKeyType)
                    {
                        row.Add(pair.Key.Type.Name);
                    }
                    row.Add(GetChunk(pair.Key));
                    row.Add(GetChunk(pair.Value));

                    if (displayValueType)
                    {
                        row.Add(pair.Value.Type.Name);
                    }

                    table.AddRow(row);
                }
            }
        }

        private Chunk GetChunk(DynamicInstance keyOrValue)
        {
            return ChunkFactory.CreateTextOrLink(
                keyOrValue,
                () => ExternalCommandNames.DumpStructOrObject(keyOrValue));
        }

        public override void WriteHelp()
        {
            Output.WriteLine("-------------------------------------------------------------------------------");
            Output.WriteLine($"!{nameof(DumpObj)} <object address>");
            Output.WriteLine();
            Output.WriteLine("This command allows you to examine the fields of an object.");
            Output.WriteLine("If an object is dictionary then in addition to the fields of an object");
            Output.WriteLine("a list of its elements will also be displayed (in form key-value)");
            Output.WriteLine($"Usage example:");
            Output.WriteLine(
                @"
0:000 > !{0} 2cf4174
Name:        System.Collections.Generic.Dictionary`2[[System.Object, mscorlib],[System.Object, mscorlib]]
MethodTable: 6f89e4d4
EEClass:     6f914ec0
Size:        48(0x30) bytes
File:        C:\Windows\Microsoft.Net\assembly\GAC_32\mscorlib\v4.0_4.0.0.0__b77a5c561934e089\mscorlib.dll
Fields:
      MT    Field   Offset                 Type VT     Attr    Value Name
6fd3f4f0  4001871        4       System.Int32[]  0 instance 02cf4278 buckets
707af224  4001872        8 ...non, mscorlib]][]  0 instance 02cf42a0 entries
6fd3f52c  4001873       1c         System.Int32  1 instance        4 count
6fd3f52c  4001874       20         System.Int32  1 instance        4 version
6fd3f52c  4001875       24         System.Int32  1 instance       -1 freeList
6fd3f52c  4001876       28         System.Int32  1 instance        0 freeCount
6fd7df50  4001877        c ...Canon, mscorlib]]  0 instance 02cf41c0 comparer
6fd90584  4001878       10 ...Canon, mscorlib]]  0 instance 00000000 keys
6fd49094  4001879       14 ...Canon, mscorlib]]  0 instance 00000000 values
6fd3da78  400187a       18        System.Object  0 instance 00000000 _syncRoot

Items[4]:

 | KeyType             |              Key | Value    | ValueType               | 
 | ------------------- | ---------------- | -------- | ----------------------- | 
 | System.String       | ""this is string"" | 1234     | System.Int32            | 
 | SampleApp.MyClass1  |         02cf422c | 02cf4238 | SampleApp.DerivedClass1 | 
 | SampleApp.MyEnum    |       EnumValue1 | 12.345   | System.Double           | 
 | SampleApp.MyStruct1 |         02cf4264 | ""string"" | System.String           | ", nameof(DumpObj));
        }
    }
}
