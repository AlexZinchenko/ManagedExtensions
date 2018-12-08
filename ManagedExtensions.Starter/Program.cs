using ManagedExtensions.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ManagedExtensions.Core.Helpers;

namespace ManagedExtensions.Starter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string fileName = null;

            if (TryGetFileFromArgs(args, out fileName) 
             || TryChooseDump(out fileName))
            {
                OpenDump(fileName);
            }
        }

        private static bool TryGetFileFromArgs(string[] args, out string fileName)
        {
            if (args.Length == 1)
            {
                fileName = args[0];
                return true;
            }

            fileName = null;
            return false;
        }

        private static bool TryChooseDump(out string fileName)
        {
            var openFileDlg = new OpenFileDialog
            {
                Filter = "dmp (*.dmp)|*.dmp"
            };

            var res = (openFileDlg.ShowDialog() == DialogResult.OK);
            fileName = openFileDlg.FileName;

            return res;
        }

        private static void OpenDump(string fileName)
        {
            var startupCommand = "!dumpdps 17e5ad18";
            var winDbgCommandsFile = GetCommandsTempFile(
                startCommands.Concat(new []
                {
                    GetLoadExtensionCommand(),
                    startupCommand
                }));

            Process
                .Start("windbg.exe", $@"-z ""{fileName}"" -c ""$<{winDbgCommandsFile}""")
                .WaitForExit();
        }

        private static string GetCommandsTempFile(IEnumerable<string> commands)
        {
            return TempFile.Create(string.Join("\n", commands), "txt");
        }

        private static string GetLoadExtensionCommand()
        {
            var extensionAssembly = typeof(EntryPoint).Assembly.Location;
            return $".load {extensionAssembly}";
        }

        private static string[] startCommands = new string[]
        {
            ".prefer_dml 1",
            ".symfix",
            ".cordll -ve -u -l"
            //"!bhi"
        };
    }
}
