using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedExtensions.Core;
using ManagedExtensions.Core.Commands;
using ManagedExtensions.Core.Out;

namespace ManagedExtensions.CommonCommands
{
    public sealed class HelpCommand : BaseCommand
    {
        public HelpCommand(ICommandsHost debugger)
            : base(debugger)
        {
        }

        [DllExport, CommandMethod]
        public static void help(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string args)
        {
            EntryPoint.Execute<HelpCommand>(client, c => c.Help(args));
        }

        public void Help(string commandMethod)
        {
            if (string.IsNullOrEmpty(commandMethod))
            {
                WriteHelp();
            }
            else
            {
                var command = Host.FindCommandByMethodName(commandMethod);

                if (command != null)
                {
                    command.WriteHelp();
                }
                else
                {
                    Output.WriteLine("No command found. Available commands:");
                    WriteCommandGroups();
                }
            }
        }

        public string GetCallString(string commandName)
        {
            return $"!{nameof(help)} {commandName}";
        }

        public override void WriteHelp()
        {
            Output.WriteLine(
@"ManagedExtensions is a debugger extension DLL that contains some useful commands.

Extension is written in C# and works with ClrMd.
Commands are listed by category.
Shortcut names for popular commands are listed in parenthesis.
Type ""!{0}"" <command name> (or turn on dml) for detailed info on that command.", nameof(help));

            WriteCommandGroups();
        }

        private void WriteCommandGroups()
        {
            var groups = Host.Commands.AllCommands.GroupBy(c => c.Group);
            foreach (var commandGroup in groups)
            {
                Output.WriteLine();
                Output.WriteLine($"{commandGroup.Key}");
                Output.WriteLine("-----------------------------");

                foreach (var command in commandGroup)
                {
                    var names = command.ExportedNames;

                    Output.WriteLine("{0}", new Link(ConcatNames(names), GetCallString(names.First())));
                }
            }
        }

        private string ConcatNames(List<string> names)
        {
            var concattedNames = names.First();

            if (names.Count > 1)
                concattedNames += string.Format(" ({0})", string.Join(", ", names.Skip(1)));

            return concattedNames;
        }
    }
}
