using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ManagedExtensions.Core;
using ManagedExtensions.Core.Commands;

namespace ManagedExtensions.CommonCommands
{
    public sealed class OpenFileCommand : NativeCommand
    {
        public OpenFileCommand(INativeCommandsHost debugger)
            : base(debugger)
        {
        }

        [DllExport, CommandMethod]
        public static void openfile(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<OpenFileCommand>(client, c => c.OpenFile(args));
        }

        public void OpenFile(string filePath)
        {
            Process.Start(filePath);
        }

        public string GetCallString(string filePath)
        {
            return $"!{nameof(openfile)} {filePath}";
        }

        public override void WriteHelp()
        {
            Output.WriteLine("-------------------------------------------------------------------------------");
            Output.WriteLine($"!{nameof(openfile)} <file path>");
            Output.WriteLine();
            Output.WriteLine("This command opens specified file like you click on it in explorer");
            Output.WriteLine($"Usage example:");
            Output.WriteLine();
            Output.WriteLine($"\t!{nameof(openfile)} D:\\log.txt");
        }
    }
}
